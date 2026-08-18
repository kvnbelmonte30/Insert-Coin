using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Cierres;
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
[Route("api/cierres-diarios")]
[Authorize]
public class CierresDiariosController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public CierresDiariosController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet("local/{localId:guid}")]
    public async Task<ActionResult<IEnumerable<CierreDiarioDto>>> GetByLocal(
        Guid localId,
        [FromQuery] DateOnly? desde = null,
        [FromQuery] DateOnly? hasta = null)
    {
        if (!CurrentUser.HasAccessToLocal(User, localId)) return Forbid();

        var query = BaseQuery().Where(c => c.LocalId == localId);
        if (desde is not null) query = query.Where(c => c.Fecha >= desde);
        if (hasta is not null) query = query.Where(c => c.Fecha <= hasta);

        var cierres = await query.OrderByDescending(c => c.Fecha).ToListAsync();
        return Ok(cierres.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CierreDiarioDto>> GetById(Guid id)
    {
        var cierre = await BaseQuery().FirstOrDefaultAsync(c => c.Id == id);
        if (cierre is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, cierre.LocalId)) return Forbid();

        return Ok(ToDto(cierre));
    }

    [HttpPost("local/{localId:guid}")]
    [Authorize(Roles = Roles.Empleado)]
    public async Task<ActionResult<CierreDiarioDto>> Registrar(Guid localId, RegistrarCierreDiarioRequest request)
    {
        if (!CurrentUser.HasAccessToLocal(User, localId)) return Forbid();

        var semana = await _db.Semanas
            .Where(s => s.LocalId == localId && s.Estado != EstadoSemana.Cerrada)
            .OrderByDescending(s => s.Numero)
            .FirstOrDefaultAsync();
        if (semana is null) return BadRequest(new { message = "El local no tiene una semana activa." });

        var cuentaActual = await _db.Cuentas
            .Include(c => c.Detalles)
            .Where(c => c.LocalId == localId && c.Activa)
            .OrderByDescending(c => c.FechaCreacion)
            .FirstOrDefaultAsync();
        if (cuentaActual is null) return BadRequest(new { message = "El local no tiene una cuenta configurada." });

        var cierre = new CierreDiario
        {
            LocalId = localId,
            SemanaId = semana.Id,
            EmpleadoId = CurrentUser.GetId(User),
            Fecha = request.Fecha
        };

        foreach (var linea in request.Lineas)
        {
            var (valorUnitario, cantidad) = await ResolverLinea(linea);
            cierre.Detalles.Add(new CierreDiarioDetalle
            {
                Concepto = linea.Concepto,
                DenominacionId = linea.DenominacionId,
                PremioId = linea.PremioId,
                Cantidad = cantidad,
                ValorUnitario = valorUnitario
            });
        }

        if (request.GastoIds.Count > 0)
        {
            var gastos = await _db.Gastos.Where(g => request.GastoIds.Contains(g.Id) && g.LocalId == localId).ToListAsync();
            foreach (var gasto in gastos)
            {
                gasto.CierreDiarioId = cierre.Id;
                cierre.Detalles.Add(new CierreDiarioDetalle
                {
                    Concepto = ConceptoMovimiento.Gasto,
                    Cantidad = 1,
                    ValorUnitario = gasto.Monto
                });
            }
        }

        cierre.TotalReportado = cierre.Detalles.Sum(d => d.Subtotal);
        cierre.TotalEsperado = cuentaActual.Detalles.Sum(d => d.Subtotal);
        cierre.Estado = cierre.TotalReportado == cierre.TotalEsperado ? EstadoCierre.Correcto : EstadoCierre.Revisar;

        _db.CierresDiarios.Add(cierre);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), localId,
            "Cierre diario realizado", nameof(CierreDiario), cierre.Id.ToString(), null,
            new { cierre.TotalReportado, cierre.TotalEsperado, cierre.Estado });

        var cierreCompleto = await BaseQuery().FirstAsync(c => c.Id == cierre.Id);
        return Ok(ToDto(cierreCompleto));
    }

    private async Task<(decimal valorUnitario, int cantidad)> ResolverLinea(LineaCierreRequest linea)
    {
        if (linea.Concepto is ConceptoMovimiento.Terminal or ConceptoMovimiento.Transferencia)
        {
            return (linea.Monto ?? 0, 1);
        }

        if (linea.DenominacionId is not null)
        {
            var d = await _db.Denominaciones.FindAsync(linea.DenominacionId.Value);
            if (d is null) throw new InvalidOperationException("Denominación no encontrada.");
            var valor = d.Tipo == TipoDenominacion.Bolsa ? (d.ValorPorBolsa ?? 0) : d.Valor;
            return (valor, linea.Cantidad);
        }

        if (linea.PremioId is not null)
        {
            var p = await _db.Premios.FindAsync(linea.PremioId.Value);
            if (p is null) throw new InvalidOperationException("Premio no encontrado.");
            return (p.Denominacion, linea.Cantidad);
        }

        throw new InvalidOperationException("Línea inválida: falta denominación, premio o monto.");
    }

    private IQueryable<CierreDiario> BaseQuery() => _db.CierresDiarios
        .Include(c => c.Empleado)
        .Include(c => c.Detalles).ThenInclude(d => d.Denominacion)
        .Include(c => c.Detalles).ThenInclude(d => d.Premio);

    private static CierreDiarioDto ToDto(CierreDiario cierre) => new()
    {
        Id = cierre.Id,
        LocalId = cierre.LocalId,
        Fecha = cierre.Fecha,
        EmpleadoNombre = cierre.Empleado.Nombre,
        TotalReportado = cierre.TotalReportado,
        TotalEsperado = cierre.TotalEsperado,
        Diferencia = cierre.Diferencia,
        Estado = cierre.Estado,
        FechaCreacion = cierre.FechaCreacion,
        Detalles = cierre.Detalles.Select(d => new CierreDiarioDetalleDto
        {
            Concepto = d.Concepto,
            DenominacionNombre = d.Denominacion is null ? null : $"{d.Denominacion.Tipo} ${d.Denominacion.Valor:0.##}",
            PremioNombre = d.Premio?.Nombre,
            Cantidad = d.Cantidad,
            ValorUnitario = d.ValorUnitario,
            Subtotal = d.Subtotal
        }).ToList()
    };
}
