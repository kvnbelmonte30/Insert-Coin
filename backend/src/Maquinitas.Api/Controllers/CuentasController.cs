using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Cuentas;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cuentas;
using Maquinitas.Domain.Entities.Notificaciones;
using Maquinitas.Domain.Entities.Semanas;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CuentasController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public CuentasController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet("local/{localId:guid}/actual")]
    public async Task<ActionResult<CuentaDto>> GetActual(Guid localId)
    {
        if (!CurrentUser.HasAccessToLocal(User, localId)) return Forbid();

        var cuenta = await CuentaActualQuery(localId).FirstOrDefaultAsync();
        if (cuenta is null) return NotFound(new { message = "El local aún no tiene una cuenta configurada para la semana actual." });

        return Ok(ToDto(cuenta));
    }

    [HttpPost("local/{localId:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<CuentaDto>> Crear(Guid localId, CrearCuentaRequest request)
    {
        var semana = await _db.Semanas
            .Where(s => s.LocalId == localId && s.Estado != EstadoSemana.Cerrada)
            .OrderByDescending(s => s.Numero)
            .FirstOrDefaultAsync();

        if (semana is null) return BadRequest(new { message = "El local no tiene una semana activa." });

        var yaExiste = await _db.Cuentas.AnyAsync(c => c.SemanaId == semana.Id && c.Activa);
        if (yaExiste) return Conflict(new { message = "Ya existe una cuenta activa para la semana actual. Usa la edición de líneas para modificarla." });

        var cuenta = new Cuenta
        {
            LocalId = localId,
            SemanaId = semana.Id,
            CreadoPorUsuarioId = CurrentUser.GetId(User)
        };

        foreach (var linea in request.Lineas)
        {
            var (valorUnitario, _) = await ResolverValorUnitario(linea.DenominacionId, linea.PremioId);
            cuenta.Detalles.Add(new CuentaDetalle
            {
                DenominacionId = linea.DenominacionId,
                PremioId = linea.PremioId,
                Cantidad = linea.Cantidad,
                ValorUnitario = valorUnitario,
                Origen = "Configuración inicial",
                RegistradoPorUsuarioId = CurrentUser.GetId(User)
            });
        }

        _db.Cuentas.Add(cuenta);
        await _db.SaveChangesAsync();

        var cuentaCompleta = await CuentaActualQuery(localId).FirstAsync(c => c.Id == cuenta.Id);

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), localId,
            "Cuenta creada", nameof(Cuenta), cuenta.Id.ToString(), null, ToDto(cuentaCompleta));

        return Ok(ToDto(cuentaCompleta));
    }

    [HttpPost("{cuentaId:guid}/lineas")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<CuentaDto>> AgregarLinea(Guid cuentaId, LineaCuentaRequest request)
    {
        var cuenta = await _db.Cuentas.FirstOrDefaultAsync(c => c.Id == cuentaId);
        if (cuenta is null) return NotFound();

        var (valorUnitario, concepto) = await ResolverValorUnitario(request.DenominacionId, request.PremioId);

        var detalle = new CuentaDetalle
        {
            CuentaId = cuentaId,
            DenominacionId = request.DenominacionId,
            PremioId = request.PremioId,
            Cantidad = request.Cantidad,
            ValorUnitario = valorUnitario,
            Origen = "Ajuste administrador",
            RegistradoPorUsuarioId = CurrentUser.GetId(User)
        };
        _db.CuentaDetalles.Add(detalle);

        await RegistrarModificacionYNotificar(cuenta, $"{concepto} agregado", "0", request.Cantidad.ToString());

        await _db.SaveChangesAsync();

        var cuentaCompleta = await CuentaActualQuery(cuenta.LocalId).FirstAsync(c => c.Id == cuentaId);
        return Ok(ToDto(cuentaCompleta));
    }

    [HttpPut("lineas/{detalleId:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<CuentaDto>> ActualizarLinea(Guid detalleId, ActualizarCantidadRequest request)
    {
        var detalle = await _db.CuentaDetalles
            .Include(d => d.Denominacion)
            .Include(d => d.Premio)
            .Include(d => d.Cuenta)
            .FirstOrDefaultAsync(d => d.Id == detalleId);
        if (detalle is null) return NotFound();

        var concepto = detalle.Denominacion is not null
            ? $"{detalle.Denominacion.Tipo} ${detalle.Denominacion.Valor:0.##}"
            : detalle.Premio?.Nombre ?? "Línea de cuenta";

        var valorAnterior = detalle.Cantidad.ToString();
        detalle.Cantidad = request.Cantidad;

        await RegistrarModificacionYNotificar(detalle.Cuenta, concepto, valorAnterior, request.Cantidad.ToString());

        await _db.SaveChangesAsync();

        var cuentaCompleta = await CuentaActualQuery(detalle.Cuenta.LocalId).FirstAsync(c => c.Id == detalle.CuentaId);
        return Ok(ToDto(cuentaCompleta));
    }

    [HttpGet("{cuentaId:guid}/modificaciones")]
    public async Task<ActionResult<IEnumerable<CuentaModificacionDto>>> GetModificaciones(Guid cuentaId)
    {
        var cuenta = await _db.Cuentas.FirstOrDefaultAsync(c => c.Id == cuentaId);
        if (cuenta is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, cuenta.LocalId)) return Forbid();

        var modificaciones = await _db.CuentaModificacionLogs
            .Include(m => m.Usuario)
            .Where(m => m.CuentaId == cuentaId)
            .OrderByDescending(m => m.Fecha)
            .Select(m => new CuentaModificacionDto
            {
                Id = m.Id,
                UsuarioNombre = m.Usuario.Nombre,
                Concepto = m.Concepto,
                ValorAnterior = m.ValorAnterior,
                ValorNuevo = m.ValorNuevo,
                Fecha = m.Fecha
            })
            .ToListAsync();

        return Ok(modificaciones);
    }

    private async Task RegistrarModificacionYNotificar(Cuenta cuenta, string concepto, string valorAnterior, string valorNuevo)
    {
        _db.CuentaModificacionLogs.Add(new CuentaModificacionLog
        {
            CuentaId = cuenta.Id,
            UsuarioId = CurrentUser.GetId(User),
            Concepto = concepto,
            ValorAnterior = valorAnterior,
            ValorNuevo = valorNuevo,
            NotificacionEnviada = true
        });

        var empleadosDelLocal = await _db.UsuarioLocales
            .Where(ul => ul.LocalId == cuenta.LocalId)
            .Select(ul => ul.UsuarioId)
            .ToListAsync();

        foreach (var usuarioId in empleadosDelLocal)
        {
            _db.Set<Notificacion>().Add(new Notificacion
            {
                UsuarioId = usuarioId,
                LocalId = cuenta.LocalId,
                Tipo = TipoNotificacion.CuentaModificada,
                Mensaje = $"El administrador modificó la cuenta: {concepto} ({valorAnterior} → {valorNuevo}).",
                ReferenciaId = cuenta.Id
            });
        }

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), cuenta.LocalId,
            "Cuenta modificada", nameof(Cuenta), cuenta.Id.ToString(), valorAnterior, valorNuevo);
    }

    private async Task<(decimal valorUnitario, string concepto)> ResolverValorUnitario(Guid? denominacionId, Guid? premioId)
    {
        if (denominacionId is not null)
        {
            var d = await _db.Denominaciones.FindAsync(denominacionId.Value);
            if (d is null) throw new InvalidOperationException("Denominación no encontrada.");
            var valor = d.Tipo == TipoDenominacion.Bolsa ? (d.ValorPorBolsa ?? 0) : d.Valor;
            return (valor, $"{d.Tipo} ${d.Valor:0.##}");
        }

        if (premioId is not null)
        {
            var p = await _db.Premios.FindAsync(premioId.Value);
            if (p is null) throw new InvalidOperationException("Premio no encontrado.");
            return (p.Denominacion, p.Nombre);
        }

        throw new InvalidOperationException("Debe indicar una denominación o un premio.");
    }

    private IQueryable<Cuenta> CuentaActualQuery(Guid localId) => _db.Cuentas
        .Include(c => c.Semana)
        .Include(c => c.CreadoPorUsuario)
        .Include(c => c.Detalles).ThenInclude(d => d.Denominacion)
        .Include(c => c.Detalles).ThenInclude(d => d.Premio)
        .Where(c => c.LocalId == localId && c.Activa)
        .OrderByDescending(c => c.FechaCreacion);

    private static CuentaDto ToDto(Cuenta cuenta) => new()
    {
        Id = cuenta.Id,
        LocalId = cuenta.LocalId,
        SemanaId = cuenta.SemanaId,
        SemanaNumero = cuenta.Semana.Numero,
        FechaCreacion = cuenta.FechaCreacion,
        CreadoPorNombre = cuenta.CreadoPorUsuario.Nombre,
        Detalles = cuenta.Detalles.Select(d => new CuentaDetalleDto
        {
            Id = d.Id,
            DenominacionId = d.DenominacionId,
            DenominacionNombre = d.Denominacion is null ? null : $"{d.Denominacion.Tipo} ${d.Denominacion.Valor:0.##}",
            PremioId = d.PremioId,
            PremioNombre = d.Premio?.Nombre,
            Cantidad = d.Cantidad,
            ValorUnitario = d.ValorUnitario,
            Subtotal = d.Subtotal,
            Origen = d.Origen
        }).ToList(),
        TotalAcumulado = cuenta.Detalles.Sum(d => d.Subtotal)
    };
}
