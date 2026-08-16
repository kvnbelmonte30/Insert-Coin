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
public class MaquinasController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly AuditoriaService _auditoria;

    public MaquinasController(ApplicationDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    [HttpGet("tipos")]
    public async Task<ActionResult<IEnumerable<TipoMaquinaDto>>> GetTipos()
    {
        var tipos = await _db.TiposMaquina.Where(t => t.Activo).OrderBy(t => t.Nombre).ToListAsync();
        return Ok(tipos.Select(t => new TipoMaquinaDto { Id = t.Id, Nombre = t.Nombre, Activo = t.Activo }));
    }

    [HttpPost("tipos")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<TipoMaquinaDto>> CrearTipo(CrearTipoMaquinaRequest request)
    {
        var tipo = new TipoMaquina { Nombre = request.Nombre };
        _db.TiposMaquina.Add(tipo);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Tipo de máquina creado", nameof(TipoMaquina), tipo.Id.ToString(), null, tipo.Nombre);

        return Ok(new TipoMaquinaDto { Id = tipo.Id, Nombre = tipo.Nombre, Activo = tipo.Activo });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaquinaDto>>> GetAll([FromQuery] Guid? localId, [FromQuery] Guid? propietarioId)
    {
        var query = BaseQuery();

        if (localId is not null)
        {
            if (!CurrentUser.HasAccessToLocal(User, localId.Value)) return Forbid();
            query = query.Where(m => m.LocalId == localId);
        }
        else if (!CurrentUser.IsAdmin(User))
        {
            var localIds = CurrentUser.GetLocalIds(User);
            query = query.Where(m => localIds.Contains(m.LocalId));
        }

        if (propietarioId is not null) query = query.Where(m => m.PropietarioId == propietarioId);

        var maquinas = await query.OrderBy(m => m.Nombre).ToListAsync();
        return Ok(maquinas.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MaquinaDto>> GetById(Guid id)
    {
        var maquina = await BaseQuery().FirstOrDefaultAsync(m => m.Id == id);
        if (maquina is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, maquina.LocalId)) return Forbid();

        return Ok(ToDto(maquina));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<MaquinaDto>> Create(CrearMaquinaRequest request)
    {
        var maquina = new Maquina
        {
            Nombre = request.Nombre,
            TipoMaquinaId = request.TipoMaquinaId,
            LocalId = request.LocalId,
            PropietarioId = request.PropietarioId,
            Marca = request.Marca,
            Modelo = request.Modelo,
            NumeroSerie = request.NumeroSerie
        };

        _db.Maquinas.Add(maquina);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), request.LocalId,
            "Máquina creada", nameof(Maquina), maquina.Id.ToString(), null, new { maquina.Nombre });

        var completa = await BaseQuery().FirstAsync(m => m.Id == maquina.Id);
        return Ok(ToDto(completa));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<MaquinaDto>> Update(Guid id, ActualizarMaquinaRequest request)
    {
        var maquina = await _db.Maquinas.FindAsync(id);
        if (maquina is null) return NotFound();

        var anterior = new { maquina.Nombre, maquina.PropietarioId, maquina.Marca, maquina.Modelo, maquina.Activo };

        maquina.Nombre = request.Nombre;
        maquina.PropietarioId = request.PropietarioId;
        maquina.Marca = request.Marca;
        maquina.Modelo = request.Modelo;
        maquina.NumeroSerie = request.NumeroSerie;
        maquina.Activo = request.Activo;
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), maquina.LocalId,
            "Máquina modificada", nameof(Maquina), maquina.Id.ToString(), anterior,
            new { maquina.Nombre, maquina.PropietarioId, maquina.Marca, maquina.Modelo, maquina.Activo });

        var completa = await BaseQuery().FirstAsync(m => m.Id == maquina.Id);
        return Ok(ToDto(completa));
    }

    [HttpPut("{id:guid}/estado")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<MaquinaDto>> CambiarEstado(Guid id, CambiarEstadoMaquinaRequest request)
    {
        var maquina = await _db.Maquinas.FindAsync(id);
        if (maquina is null) return NotFound();

        var estadoAnterior = maquina.Estado;
        maquina.Estado = request.Estado;

        _db.HistorialEstadosMaquina.Add(new HistorialEstadoMaquina
        {
            MaquinaId = maquina.Id,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = request.Estado,
            UsuarioId = CurrentUser.GetId(User),
            Comentario = request.Comentario
        });

        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), maquina.LocalId,
            "Estado de máquina cambiado", nameof(Maquina), maquina.Id.ToString(), estadoAnterior, request.Estado);

        var completa = await BaseQuery().FirstAsync(m => m.Id == maquina.Id);
        return Ok(ToDto(completa));
    }

    [HttpGet("{id:guid}/historial")]
    public async Task<ActionResult<IEnumerable<HistorialEstadoDto>>> GetHistorial(Guid id)
    {
        var maquina = await _db.Maquinas.FindAsync(id);
        if (maquina is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, maquina.LocalId)) return Forbid();

        var historial = await _db.HistorialEstadosMaquina
            .Include(h => h.Usuario)
            .Where(h => h.MaquinaId == id)
            .OrderByDescending(h => h.Fecha)
            .Select(h => new HistorialEstadoDto
            {
                EstadoAnterior = h.EstadoAnterior,
                EstadoNuevo = h.EstadoNuevo,
                UsuarioNombre = h.Usuario.Nombre,
                Comentario = h.Comentario,
                Fecha = h.Fecha
            })
            .ToListAsync();

        return Ok(historial);
    }

    private IQueryable<Maquina> BaseQuery() => _db.Maquinas
        .Include(m => m.TipoMaquina)
        .Include(m => m.Local)
        .Include(m => m.Propietario);

    private static MaquinaDto ToDto(Maquina m) => new()
    {
        Id = m.Id,
        Nombre = m.Nombre,
        TipoMaquinaId = m.TipoMaquinaId,
        TipoMaquinaNombre = m.TipoMaquina.Nombre,
        LocalId = m.LocalId,
        LocalNombre = m.Local.Nombre,
        PropietarioId = m.PropietarioId,
        PropietarioNombre = m.Propietario.Nombre,
        Marca = m.Marca,
        Modelo = m.Modelo,
        NumeroSerie = m.NumeroSerie,
        Estado = m.Estado,
        FechaAlta = m.FechaAlta,
        Activo = m.Activo
    };
}
