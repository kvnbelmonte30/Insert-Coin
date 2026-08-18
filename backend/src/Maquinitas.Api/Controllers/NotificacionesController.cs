using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Notificaciones;
using Maquinitas.Domain.Entities.Notificaciones;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public NotificacionesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> GetMias([FromQuery] int limite = 30)
    {
        var usuarioId = CurrentUser.GetId(User);

        var notificaciones = await _db.Notificaciones
            .Include(n => n.Local)
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.FechaCreacion)
            .Take(limite)
            .ToListAsync();

        return Ok(notificaciones.Select(ToDto));
    }

    [HttpGet("no-leidas-count")]
    public async Task<ActionResult<int>> GetNoLeidasCount()
    {
        var usuarioId = CurrentUser.GetId(User);
        var count = await _db.Notificaciones.CountAsync(n => n.UsuarioId == usuarioId && !n.Leida);
        return Ok(count);
    }

    [HttpPut("{id:guid}/leer")]
    public async Task<IActionResult> MarcarLeida(Guid id)
    {
        var usuarioId = CurrentUser.GetId(User);
        var notificacion = await _db.Notificaciones.FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == usuarioId);
        if (notificacion is null) return NotFound();

        notificacion.Leida = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("leer-todas")]
    public async Task<IActionResult> MarcarTodasLeidas()
    {
        var usuarioId = CurrentUser.GetId(User);
        var pendientes = await _db.Notificaciones.Where(n => n.UsuarioId == usuarioId && !n.Leida).ToListAsync();
        foreach (var n in pendientes) n.Leida = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static NotificacionDto ToDto(Notificacion n) => new()
    {
        Id = n.Id,
        LocalId = n.LocalId,
        LocalNombre = n.Local?.Nombre,
        Tipo = n.Tipo,
        Mensaje = n.Mensaje,
        ReferenciaId = n.ReferenciaId,
        Leida = n.Leida,
        FechaCreacion = n.FechaCreacion
    };
}
