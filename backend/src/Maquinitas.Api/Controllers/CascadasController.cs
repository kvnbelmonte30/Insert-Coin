using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Cascadas;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Premios;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/maquinas/{maquinaId:guid}")]
[Authorize]
public class CascadasController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public CascadasController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet("premios")]
    public async Task<ActionResult<IEnumerable<MaquinaPremioDto>>> GetConfiguracion(Guid maquinaId)
    {
        var maquina = await _db.Maquinas.FindAsync(maquinaId);
        if (maquina is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, maquina.LocalId)) return Forbid();

        var config = await _db.MaquinaPremios
            .Include(mp => mp.Premio)
            .Where(mp => mp.MaquinaId == maquinaId && mp.Activo)
            .OrderByDescending(mp => mp.Premio.Denominacion)
            .ToListAsync();

        return Ok(config.Select(ToDto));
    }

    [HttpPost("premios")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<MaquinaPremioDto>> ConfigurarPremio(Guid maquinaId, ConfigurarPremioRequest request)
    {
        var maquina = await _db.Maquinas.FindAsync(maquinaId);
        if (maquina is null) return NotFound();

        var existente = await _db.MaquinaPremios
            .Include(mp => mp.Premio)
            .FirstOrDefaultAsync(mp => mp.MaquinaId == maquinaId && mp.PremioId == request.PremioId);

        if (existente is not null)
        {
            var anterior = existente.CantidadAsignada;
            existente.CantidadAsignada = request.CantidadAsignada;
            existente.Activo = true;
            await _db.SaveChangesAsync();

            await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), maquina.LocalId,
                "Premio configurado", nameof(MaquinaPremio), existente.Id.ToString(), anterior, request.CantidadAsignada);

            return Ok(ToDto(existente));
        }

        var premio = await _db.Premios.FindAsync(request.PremioId);
        if (premio is null) return NotFound(new { message = "Premio no encontrado." });

        var nuevo = new MaquinaPremio
        {
            MaquinaId = maquinaId,
            PremioId = request.PremioId,
            CantidadAsignada = request.CantidadAsignada,
            Premio = premio
        };
        _db.MaquinaPremios.Add(nuevo);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), maquina.LocalId,
            "Premio configurado", nameof(MaquinaPremio), nuevo.Id.ToString(), null, request.CantidadAsignada);

        return Ok(ToDto(nuevo));
    }

    [HttpPost("inventarios")]
    [Authorize(Roles = Roles.Empleado)]
    public async Task<ActionResult<InventarioPremioDto>> RegistrarConteo(Guid maquinaId, ConteoInventarioRequest request)
    {
        var maquina = await _db.Maquinas.FindAsync(maquinaId);
        if (maquina is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, maquina.LocalId)) return Forbid();

        var configuraciones = await _db.MaquinaPremios
            .Where(mp => mp.MaquinaId == maquinaId && mp.Activo)
            .ToDictionaryAsync(mp => mp.PremioId, mp => mp.CantidadAsignada);

        var inventario = new InventarioPremio
        {
            MaquinaId = maquinaId,
            UsuarioId = CurrentUser.GetId(User)
        };

        foreach (var linea in request.Lineas)
        {
            inventario.Detalles.Add(new InventarioPremioDetalle
            {
                PremioId = linea.PremioId,
                CantidadConfigurada = configuraciones.GetValueOrDefault(linea.PremioId, 0),
                CantidadEncontrada = linea.CantidadEncontrada
            });
        }

        _db.InventariosPremios.Add(inventario);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), maquina.LocalId,
            "Inventario registrado", nameof(InventarioPremio), inventario.Id.ToString(), null, null);

        var completo = await _db.InventariosPremios
            .Include(i => i.Usuario)
            .Include(i => i.Detalles).ThenInclude(d => d.Premio)
            .FirstAsync(i => i.Id == inventario.Id);

        return Ok(ToDto(completo));
    }

    [HttpGet("inventarios")]
    public async Task<ActionResult<IEnumerable<InventarioPremioDto>>> GetHistorialInventarios(Guid maquinaId)
    {
        var maquina = await _db.Maquinas.FindAsync(maquinaId);
        if (maquina is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, maquina.LocalId)) return Forbid();

        var inventarios = await _db.InventariosPremios
            .Include(i => i.Usuario)
            .Include(i => i.Detalles).ThenInclude(d => d.Premio)
            .Where(i => i.MaquinaId == maquinaId)
            .OrderByDescending(i => i.Fecha)
            .ToListAsync();

        return Ok(inventarios.Select(ToDto));
    }

    private static MaquinaPremioDto ToDto(MaquinaPremio mp) => new()
    {
        Id = mp.Id,
        PremioId = mp.PremioId,
        PremioNombre = mp.Premio.Nombre,
        PremioDenominacion = mp.Premio.Denominacion,
        CantidadAsignada = mp.CantidadAsignada
    };

    private static InventarioPremioDto ToDto(InventarioPremio inventario) => new()
    {
        Id = inventario.Id,
        MaquinaId = inventario.MaquinaId,
        UsuarioNombre = inventario.Usuario.Nombre,
        Fecha = inventario.Fecha,
        Detalles = inventario.Detalles.Select(d => new InventarioPremioDetalleDto
        {
            PremioNombre = d.Premio.Nombre,
            CantidadConfigurada = d.CantidadConfigurada,
            CantidadEncontrada = d.CantidadEncontrada,
            Diferencia = d.Diferencia
        }).ToList()
    };
}
