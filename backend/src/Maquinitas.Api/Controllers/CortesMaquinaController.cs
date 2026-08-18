using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Maquinas;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Maquinas;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/maquinas/{maquinaId:guid}/cortes")]
[Authorize]
public class CortesMaquinaController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public CortesMaquinaController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CorteMaquinaDto>>> GetHistorial(
        Guid maquinaId,
        [FromQuery] DateOnly? desde = null,
        [FromQuery] DateOnly? hasta = null)
    {
        var maquina = await _db.Maquinas.FindAsync(maquinaId);
        if (maquina is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, maquina.LocalId)) return Forbid();

        var query = BaseQuery().Where(c => c.MaquinaId == maquinaId);
        if (desde is not null) query = query.Where(c => c.Fecha >= desde);
        if (hasta is not null) query = query.Where(c => c.Fecha <= hasta);

        var cortes = await query.OrderByDescending(c => c.Fecha).ThenByDescending(c => c.FechaCreacion).ToListAsync();
        return Ok(cortes.Select(ToDto));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Empleado)]
    public async Task<ActionResult<CorteMaquinaDto>> Registrar(Guid maquinaId, RegistrarCorteMaquinaRequest request)
    {
        var maquina = await _db.Maquinas.FindAsync(maquinaId);
        if (maquina is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, maquina.LocalId)) return Forbid();

        if (request.Lineas.Count == 0)
        {
            return BadRequest(new { message = "El corte debe tener al menos una línea con cantidad." });
        }

        var corte = new CorteMaquina
        {
            MaquinaId = maquinaId,
            LocalId = maquina.LocalId,
            EmpleadoId = CurrentUser.GetId(User),
            Fecha = request.Fecha,
            Comentario = request.Comentario
        };

        foreach (var linea in request.Lineas)
        {
            var (valorUnitario, cantidad) = await ResolverLinea(linea);
            corte.Detalles.Add(new CorteMaquinaDetalle
            {
                DenominacionId = linea.DenominacionId,
                PremioId = linea.PremioId,
                Cantidad = cantidad,
                ValorUnitario = valorUnitario
            });
        }

        corte.Total = corte.Detalles.Sum(d => d.Subtotal);

        _db.CortesMaquina.Add(corte);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), maquina.LocalId,
            "Corte de máquina registrado", nameof(CorteMaquina), corte.Id.ToString(), null, new { corte.Total });

        var completo = await BaseQuery().FirstAsync(c => c.Id == corte.Id);
        return Ok(ToDto(completo));
    }

    private async Task<(decimal valorUnitario, int cantidad)> ResolverLinea(LineaCorteMaquinaRequest linea)
    {
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

        throw new InvalidOperationException("Línea inválida: falta denominación o premio.");
    }

    private IQueryable<CorteMaquina> BaseQuery() => _db.CortesMaquina
        .Include(c => c.Maquina)
        .Include(c => c.Local)
        .Include(c => c.Empleado)
        .Include(c => c.Detalles).ThenInclude(d => d.Denominacion)
        .Include(c => c.Detalles).ThenInclude(d => d.Premio);

    private static CorteMaquinaDto ToDto(CorteMaquina c) => new()
    {
        Id = c.Id,
        MaquinaId = c.MaquinaId,
        MaquinaNombre = c.Maquina.Nombre,
        LocalId = c.LocalId,
        LocalNombre = c.Local.Nombre,
        EmpleadoNombre = c.Empleado.Nombre,
        Fecha = c.Fecha,
        Comentario = c.Comentario,
        Total = c.Total,
        FechaCreacion = c.FechaCreacion,
        Detalles = c.Detalles.Select(d => new CorteMaquinaDetalleDto
        {
            DenominacionNombre = d.Denominacion is null ? null : $"{d.Denominacion.Tipo} ${d.Denominacion.Valor:0.##}",
            PremioNombre = d.Premio?.Nombre,
            Cantidad = d.Cantidad,
            ValorUnitario = d.ValorUnitario,
            Subtotal = d.Subtotal
        }).ToList()
    };
}
