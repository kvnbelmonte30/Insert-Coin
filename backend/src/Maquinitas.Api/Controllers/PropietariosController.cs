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
[Route("api/[controller]")]
[Authorize]
public class PropietariosController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public PropietariosController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropietarioDto>>> GetAll([FromQuery] bool soloActivos = true)
    {
        var query = _db.Propietarios.AsQueryable();
        if (soloActivos) query = query.Where(p => p.Activo);
        var lista = await query.OrderBy(p => p.Nombre).ToListAsync();
        return Ok(lista.Select(ToDto));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<PropietarioDto>> Create(GuardarPropietarioRequest request)
    {
        var propietario = new Propietario { Nombre = request.Nombre, Telefono = request.Telefono };
        _db.Propietarios.Add(propietario);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Propietario creado", nameof(Propietario), propietario.Id.ToString(), null, ToDto(propietario));

        return Ok(ToDto(propietario));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<PropietarioDto>> Update(Guid id, GuardarPropietarioRequest request)
    {
        var propietario = await _db.Propietarios.FindAsync(id);
        if (propietario is null) return NotFound();

        var anterior = ToDto(propietario);
        propietario.Nombre = request.Nombre;
        propietario.Telefono = request.Telefono;
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Propietario modificado", nameof(Propietario), propietario.Id.ToString(), anterior, ToDto(propietario));

        return Ok(ToDto(propietario));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var propietario = await _db.Propietarios.FindAsync(id);
        if (propietario is null) return NotFound();

        propietario.Activo = false;
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Propietario desactivado", nameof(Propietario), propietario.Id.ToString(), null, null);

        return NoContent();
    }

    private static PropietarioDto ToDto(Propietario p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Telefono = p.Telefono,
        Activo = p.Activo
    };
}
