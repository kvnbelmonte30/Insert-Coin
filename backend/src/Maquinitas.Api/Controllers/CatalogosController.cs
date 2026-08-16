using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Catalogos;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cuentas;
using Maquinitas.Domain.Entities.Premios;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatalogosController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public CatalogosController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet("denominaciones")]
    public async Task<ActionResult<IEnumerable<DenominacionDto>>> GetDenominaciones([FromQuery] bool soloActivas = true)
    {
        var query = _db.Denominaciones.AsQueryable();
        if (soloActivas) query = query.Where(d => d.Activo);

        var lista = await query.OrderBy(d => d.Tipo).ThenBy(d => d.Valor).ToListAsync();
        return Ok(lista.Select(ToDto));
    }

    [HttpPost("denominaciones")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<DenominacionDto>> CrearDenominacion(GuardarDenominacionRequest request)
    {
        var denominacion = new Denominacion
        {
            Tipo = request.Tipo,
            Valor = request.Valor,
            ValorPorBolsa = request.ValorPorBolsa
        };
        _db.Denominaciones.Add(denominacion);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Denominación creada", nameof(Denominacion), denominacion.Id.ToString(), null, ToDto(denominacion));

        return Ok(ToDto(denominacion));
    }

    [HttpPut("denominaciones/{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<DenominacionDto>> ActualizarDenominacion(Guid id, GuardarDenominacionRequest request)
    {
        var denominacion = await _db.Denominaciones.FindAsync(id);
        if (denominacion is null) return NotFound();

        var anterior = ToDto(denominacion);
        denominacion.Tipo = request.Tipo;
        denominacion.Valor = request.Valor;
        denominacion.ValorPorBolsa = request.ValorPorBolsa;
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Denominación modificada", nameof(Denominacion), denominacion.Id.ToString(), anterior, ToDto(denominacion));

        return Ok(ToDto(denominacion));
    }

    [HttpDelete("denominaciones/{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> DesactivarDenominacion(Guid id)
    {
        var denominacion = await _db.Denominaciones.FindAsync(id);
        if (denominacion is null) return NotFound();

        denominacion.Activo = false;
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Denominación desactivada", nameof(Denominacion), denominacion.Id.ToString(), null, null);

        return NoContent();
    }

    [HttpGet("premios")]
    public async Task<ActionResult<IEnumerable<PremioDto>>> GetPremios([FromQuery] bool soloActivas = true)
    {
        var query = _db.Premios.AsQueryable();
        if (soloActivas) query = query.Where(p => p.Activo);

        var lista = await query.OrderByDescending(p => p.Denominacion).ToListAsync();
        return Ok(lista.Select(ToDto));
    }

    [HttpPost("premios")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<PremioDto>> CrearPremio(GuardarPremioRequest request)
    {
        var premio = new Premio { Nombre = request.Nombre, Denominacion = request.Denominacion };
        _db.Premios.Add(premio);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Premio creado", nameof(Premio), premio.Id.ToString(), null, ToDto(premio));

        return Ok(ToDto(premio));
    }

    [HttpPut("premios/{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<PremioDto>> ActualizarPremio(Guid id, GuardarPremioRequest request)
    {
        var premio = await _db.Premios.FindAsync(id);
        if (premio is null) return NotFound();

        var anterior = ToDto(premio);
        premio.Nombre = request.Nombre;
        premio.Denominacion = request.Denominacion;
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Premio modificado", nameof(Premio), premio.Id.ToString(), anterior, ToDto(premio));

        return Ok(ToDto(premio));
    }

    [HttpDelete("premios/{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> DesactivarPremio(Guid id)
    {
        var premio = await _db.Premios.FindAsync(id);
        if (premio is null) return NotFound();

        premio.Activo = false;
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Premio desactivado", nameof(Premio), premio.Id.ToString(), null, null);

        return NoContent();
    }

    private static DenominacionDto ToDto(Denominacion d) => new()
    {
        Id = d.Id,
        Tipo = d.Tipo,
        Valor = d.Valor,
        ValorPorBolsa = d.ValorPorBolsa,
        Activo = d.Activo
    };

    private static PremioDto ToDto(Premio p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Denominacion = p.Denominacion,
        Activo = p.Activo
    };
}
