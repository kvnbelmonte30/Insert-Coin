using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Locales;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Locales;
using Maquinitas.Domain.Entities.Semanas;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocalesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public LocalesController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocalDto>>> GetAll()
    {
        IQueryable<Local> query = _db.Locales.Include(l => l.Semanas);

        var visibleIds = CurrentUser.GetVisibleLocalIds(User);
        if (visibleIds is not null)
        {
            query = query.Where(l => visibleIds.Contains(l.Id));
        }

        var locales = await query.OrderBy(l => l.Nombre).ToListAsync();

        return Ok(locales.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LocalDto>> GetById(Guid id)
    {
        if (!CurrentUser.HasAccessToLocal(User, id)) return Forbid();

        var local = await _db.Locales.Include(l => l.Semanas).FirstOrDefaultAsync(l => l.Id == id);
        if (local is null) return NotFound();

        return Ok(ToDto(local));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<LocalDto>> Create(CrearLocalRequest request)
    {
        var local = new Local
        {
            Nombre = request.Nombre,
            Direccion = request.Direccion
        };

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var semana = new Semana
        {
            LocalId = local.Id,
            Numero = 1,
            FechaInicio = hoy,
            FechaFin = hoy.AddDays(6),
            Estado = EstadoSemana.Abierta
        };
        local.Semanas.Add(semana);

        _db.Locales.Add(local);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(
            CurrentUser.GetId(User), CurrentUser.GetNombre(User), local.Id,
            "Local creado", nameof(Local), local.Id.ToString(), null, ToDto(local));

        return CreatedAtAction(nameof(GetById), new { id = local.Id }, ToDto(local));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<LocalDto>> Update(Guid id, ActualizarLocalRequest request)
    {
        var local = await _db.Locales.Include(l => l.Semanas).FirstOrDefaultAsync(l => l.Id == id);
        if (local is null) return NotFound();

        var anterior = ToDto(local);

        local.Nombre = request.Nombre;
        local.Direccion = request.Direccion;
        local.Activo = request.Activo;

        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(
            CurrentUser.GetId(User), CurrentUser.GetNombre(User), local.Id,
            "Local modificado", nameof(Local), local.Id.ToString(), anterior, ToDto(local));

        return Ok(ToDto(local));
    }

    private static LocalDto ToDto(Local local)
    {
        var semanaActual = local.Semanas
            .Where(s => s.Estado != EstadoSemana.Cerrada)
            .OrderByDescending(s => s.Numero)
            .FirstOrDefault();

        return new LocalDto
        {
            Id = local.Id,
            Nombre = local.Nombre,
            Direccion = local.Direccion,
            Activo = local.Activo,
            FechaCreacion = local.FechaCreacion,
            SemanaActualId = semanaActual?.Id,
            SemanaActualNumero = semanaActual?.Numero ?? 0
        };
    }
}
