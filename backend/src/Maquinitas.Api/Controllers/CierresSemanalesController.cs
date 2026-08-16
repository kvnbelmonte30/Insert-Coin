using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.CierresSemanales;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cierres;
using Maquinitas.Domain.Entities.Semanas;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/cierres-semanales")]
[Authorize]
public class CierresSemanalesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public CierresSemanalesController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet("local/{localId:guid}")]
    public async Task<ActionResult<IEnumerable<CierreSemanalDto>>> GetByLocal(Guid localId)
    {
        if (!CurrentUser.HasAccessToLocal(User, localId)) return Forbid();

        var cierres = await BaseQuery().Where(c => c.LocalId == localId).OrderByDescending(c => c.FechaCreacion).ToListAsync();
        return Ok(cierres.Select(ToDto));
    }

    [HttpPost("semana/{semanaId:guid}")]
    [Authorize(Roles = Roles.Empleado)]
    public async Task<ActionResult<CierreSemanalDto>> Registrar(Guid semanaId, RegistrarCierreSemanalRequest request)
    {
        var semana = await _db.Semanas.FindAsync(semanaId);
        if (semana is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, semana.LocalId)) return Forbid();
        if (semana.Estado == EstadoSemana.Cerrada) return BadRequest(new { message = "La semana ya está cerrada." });

        var existente = await _db.CierresSemanales.FirstOrDefaultAsync(c => c.SemanaId == semanaId);
        if (existente is not null) return Conflict(new { message = "Ya existe un cierre semanal para esta semana. Usa la edición mientras esté pendiente." });

        var cuentaActual = await _db.Cuentas.Include(c => c.Detalles)
            .Where(c => c.LocalId == semana.LocalId && c.Activa)
            .OrderByDescending(c => c.FechaCreacion)
            .FirstOrDefaultAsync();
        if (cuentaActual is null) return BadRequest(new { message = "El local no tiene una cuenta configurada." });

        var cierre = new CierreSemanal
        {
            SemanaId = semanaId,
            LocalId = semana.LocalId,
            CreadoPorUsuarioId = CurrentUser.GetId(User)
        };

        await ArmarDetalles(cierre, request.Lineas);

        cierre.TotalReportado = cierre.Detalles.Sum(d => d.Subtotal);
        cierre.TotalEsperado = cuentaActual.Detalles.Sum(d => d.Subtotal);
        cierre.EstadoDiferencia = cierre.TotalReportado == cierre.TotalEsperado ? EstadoCierre.Correcto : EstadoCierre.Revisar;

        semana.Estado = EstadoSemana.PendienteConfirmacion;

        _db.CierresSemanales.Add(cierre);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), semana.LocalId,
            "Cierre semanal realizado", nameof(CierreSemanal), cierre.Id.ToString(), null,
            new { cierre.TotalReportado, cierre.TotalEsperado });

        var completo = await BaseQuery().FirstAsync(c => c.Id == cierre.Id);
        return Ok(ToDto(completo));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Empleado)]
    public async Task<ActionResult<CierreSemanalDto>> Editar(Guid id, RegistrarCierreSemanalRequest request)
    {
        var cierre = await _db.CierresSemanales.Include(c => c.Detalles).FirstOrDefaultAsync(c => c.Id == id);
        if (cierre is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, cierre.LocalId)) return Forbid();
        if (cierre.Confirmado) return BadRequest(new { message = "El cierre ya fue confirmado por el administrador y no puede editarse." });

        var cuentaActual = await _db.Cuentas.Include(c => c.Detalles)
            .Where(c => c.LocalId == cierre.LocalId && c.Activa)
            .OrderByDescending(c => c.FechaCreacion)
            .FirstOrDefaultAsync();
        if (cuentaActual is null) return BadRequest(new { message = "El local no tiene una cuenta configurada." });

        _db.CierreSemanalDetalles.RemoveRange(cierre.Detalles);
        cierre.Detalles.Clear();
        await ArmarDetalles(cierre, request.Lineas);

        cierre.TotalReportado = cierre.Detalles.Sum(d => d.Subtotal);
        cierre.TotalEsperado = cuentaActual.Detalles.Sum(d => d.Subtotal);
        cierre.EstadoDiferencia = cierre.TotalReportado == cierre.TotalEsperado ? EstadoCierre.Correcto : EstadoCierre.Revisar;

        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), cierre.LocalId,
            "Cierre semanal reabierto y corregido", nameof(CierreSemanal), cierre.Id.ToString(), null,
            new { cierre.TotalReportado, cierre.TotalEsperado });

        var completo = await BaseQuery().FirstAsync(c => c.Id == cierre.Id);
        return Ok(ToDto(completo));
    }

    [HttpPost("{id:guid}/confirmar")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<CierreSemanalDto>> Confirmar(Guid id)
    {
        var cierre = await _db.CierresSemanales.Include(c => c.Semana).FirstOrDefaultAsync(c => c.Id == id);
        if (cierre is null) return NotFound();
        if (cierre.Confirmado) return BadRequest(new { message = "Este cierre ya fue confirmado." });

        cierre.Confirmado = true;
        cierre.ConfirmadoPorUsuarioId = CurrentUser.GetId(User);
        cierre.FechaConfirmacion = DateTime.UtcNow;
        cierre.Semana.Estado = EstadoSemana.Cerrada;

        var cuentaAnterior = await _db.Cuentas
            .Where(c => c.LocalId == cierre.LocalId && c.Activa)
            .FirstOrDefaultAsync();
        if (cuentaAnterior is not null) cuentaAnterior.Activa = false;

        var nuevaSemana = new Semana
        {
            LocalId = cierre.LocalId,
            Numero = cierre.Semana.Numero + 1,
            FechaInicio = cierre.Semana.FechaFin.AddDays(1),
            FechaFin = cierre.Semana.FechaFin.AddDays(7),
            Estado = EstadoSemana.Abierta
        };
        _db.Semanas.Add(nuevaSemana);

        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), cierre.LocalId,
            "Cierre semanal confirmado", nameof(CierreSemanal), cierre.Id.ToString(), null,
            new { NuevaSemanaNumero = nuevaSemana.Numero });
        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), cierre.LocalId,
            "Semana creada", nameof(Semana), nuevaSemana.Id.ToString(), null, new { nuevaSemana.Numero });

        var completo = await BaseQuery().FirstAsync(c => c.Id == cierre.Id);
        return Ok(ToDto(completo));
    }

    private async Task ArmarDetalles(CierreSemanal cierre, IList<LineaCierreSemanalRequest> lineas)
    {
        foreach (var linea in lineas)
        {
            decimal valorUnitario;
            int cantidad = linea.Cantidad;

            if (linea.Concepto is ConceptoMovimiento.Terminal or ConceptoMovimiento.Transferencia)
            {
                valorUnitario = linea.Monto ?? 0;
                cantidad = 1;
            }
            else if (linea.DenominacionId is not null)
            {
                var d = await _db.Denominaciones.FindAsync(linea.DenominacionId.Value);
                if (d is null) throw new InvalidOperationException("Denominación no encontrada.");
                valorUnitario = d.Tipo == TipoDenominacion.Bolsa ? (d.ValorPorBolsa ?? 0) : d.Valor;
            }
            else if (linea.PremioId is not null)
            {
                var p = await _db.Premios.FindAsync(linea.PremioId.Value);
                if (p is null) throw new InvalidOperationException("Premio no encontrado.");
                valorUnitario = p.Denominacion;
            }
            else
            {
                throw new InvalidOperationException("Línea inválida.");
            }

            cierre.Detalles.Add(new CierreSemanalDetalle
            {
                Concepto = linea.Concepto,
                DenominacionId = linea.DenominacionId,
                PremioId = linea.PremioId,
                EsPremioPuesto = linea.EsPremioPuesto,
                Cantidad = cantidad,
                ValorUnitario = valorUnitario
            });
        }
    }

    private IQueryable<CierreSemanal> BaseQuery() => _db.CierresSemanales
        .Include(c => c.Semana)
        .Include(c => c.CreadoPorUsuario)
        .Include(c => c.ConfirmadoPorUsuario)
        .Include(c => c.Detalles).ThenInclude(d => d.Denominacion)
        .Include(c => c.Detalles).ThenInclude(d => d.Premio);

    private static CierreSemanalDto ToDto(CierreSemanal c) => new()
    {
        Id = c.Id,
        SemanaId = c.SemanaId,
        SemanaNumero = c.Semana.Numero,
        LocalId = c.LocalId,
        TotalReportado = c.TotalReportado,
        TotalEsperado = c.TotalEsperado,
        Diferencia = c.Diferencia,
        EstadoDiferencia = c.EstadoDiferencia,
        Confirmado = c.Confirmado,
        CreadoPorNombre = c.CreadoPorUsuario.Nombre,
        ConfirmadoPorNombre = c.ConfirmadoPorUsuario?.Nombre,
        FechaCreacion = c.FechaCreacion,
        FechaConfirmacion = c.FechaConfirmacion,
        Detalles = c.Detalles.Select(d => new CierreSemanalDetalleDto
        {
            Concepto = d.Concepto,
            DenominacionNombre = d.Denominacion is null ? null : $"{d.Denominacion.Tipo} ${d.Denominacion.Valor:0.##}",
            PremioNombre = d.Premio?.Nombre,
            EsPremioPuesto = d.EsPremioPuesto,
            Cantidad = d.Cantidad,
            ValorUnitario = d.ValorUnitario,
            Subtotal = d.Subtotal
        }).ToList()
    };
}
