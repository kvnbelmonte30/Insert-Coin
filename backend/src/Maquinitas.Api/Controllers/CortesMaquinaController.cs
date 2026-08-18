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
[Authorize(Roles = Roles.Administrador)]
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
    public async Task<ActionResult<CorteMaquinaDto>> Registrar(Guid maquinaId, RegistrarCorteMaquinaRequest request)
    {
        var maquina = await _db.Maquinas.FindAsync(maquinaId);
        if (maquina is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, maquina.LocalId)) return Forbid();

        if (request.Total <= 0)
        {
            return BadRequest(new { message = "El total del corte debe ser mayor a cero." });
        }

        var corte = new CorteMaquina
        {
            MaquinaId = maquinaId,
            LocalId = maquina.LocalId,
            RegistradoPorId = CurrentUser.GetId(User),
            Fecha = request.Fecha,
            Comentario = request.Comentario,
            Total = request.Total
        };

        _db.CortesMaquina.Add(corte);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), maquina.LocalId,
            "Corte de máquina registrado", nameof(CorteMaquina), corte.Id.ToString(), null, new { corte.Total });

        var completo = await BaseQuery().FirstAsync(c => c.Id == corte.Id);
        return Ok(ToDto(completo));
    }

    private IQueryable<CorteMaquina> BaseQuery() => _db.CortesMaquina
        .Include(c => c.Maquina)
        .Include(c => c.Local)
        .Include(c => c.RegistradoPor);

    private static CorteMaquinaDto ToDto(CorteMaquina c) => new()
    {
        Id = c.Id,
        MaquinaId = c.MaquinaId,
        MaquinaNombre = c.Maquina.Nombre,
        LocalId = c.LocalId,
        LocalNombre = c.Local.Nombre,
        RegistradoPorNombre = c.RegistradoPor.Nombre,
        Fecha = c.Fecha,
        Comentario = c.Comentario,
        Total = c.Total,
        FechaCreacion = c.FechaCreacion
    };
}
