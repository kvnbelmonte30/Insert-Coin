using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Gastos;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Gastos;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GastosController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly FileStorageService _fileStorage;
    private readonly AuditoriaService _auditoria;

    public GastosController(ApplicationDbContext db, FileStorageService fileStorage, AuditoriaService auditoria)
    {
        _db = db;
        _fileStorage = fileStorage;
        _auditoria = auditoria;
    }

    [HttpGet("local/{localId:guid}")]
    public async Task<ActionResult<IEnumerable<GastoDto>>> GetByLocal(
        Guid localId,
        [FromQuery] bool soloSinCierre = false,
        [FromQuery] DateOnly? desde = null,
        [FromQuery] DateOnly? hasta = null)
    {
        if (!CurrentUser.HasAccessToLocal(User, localId)) return Forbid();

        var query = BaseQuery().Where(g => g.LocalId == localId);
        if (soloSinCierre) query = query.Where(g => g.CierreDiarioId == null);
        if (desde is not null) query = query.Where(g => g.Fecha >= desde);
        if (hasta is not null) query = query.Where(g => g.Fecha <= hasta);

        var gastos = await query.OrderByDescending(g => g.FechaRegistro).ToListAsync();
        return Ok(gastos.Select(ToDto));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Empleado)]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<GastoDto>> Crear([FromForm] CrearGastoRequest request, [FromForm] List<IFormFile> evidencias)
    {
        if (!CurrentUser.HasAccessToLocal(User, request.LocalId)) return Forbid();

        var categoriaExiste = await _db.CategoriasGasto.AnyAsync(c => c.Id == request.CategoriaGastoId);
        if (!categoriaExiste) return BadRequest(new { message = "Categoría de gasto inválida." });

        var gasto = new Gasto
        {
            LocalId = request.LocalId,
            EmpleadoId = CurrentUser.GetId(User),
            Descripcion = request.Descripcion,
            CategoriaGastoId = request.CategoriaGastoId,
            Monto = request.Monto,
            Fecha = request.Fecha
        };

        foreach (var archivo in evidencias.Where(a => a.Length > 0))
        {
            var ruta = await _fileStorage.GuardarAsync($"gastos/{gasto.Id}", archivo);
            gasto.Evidencias.Add(new Evidencia { RutaArchivo = ruta, NombreArchivo = archivo.FileName });
        }

        _db.Gastos.Add(gasto);
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), request.LocalId,
            "Gasto registrado", nameof(Gasto), gasto.Id.ToString(), null, new { gasto.Descripcion, gasto.Monto });

        var completo = await BaseQuery().FirstAsync(g => g.Id == gasto.Id);
        return Ok(ToDto(completo));
    }

    [HttpGet("evidencias/{evidenciaId:guid}")]
    public async Task<IActionResult> GetEvidencia(Guid evidenciaId)
    {
        var evidencia = await _db.Evidencias.Include(e => e.Gasto).FirstOrDefaultAsync(e => e.Id == evidenciaId);
        if (evidencia is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, evidencia.Gasto.LocalId)) return Forbid();

        var (stream, contentType) = _fileStorage.Abrir(evidencia.RutaArchivo);
        return File(stream, contentType);
    }

    private IQueryable<Gasto> BaseQuery() => _db.Gastos
        .Include(g => g.Empleado)
        .Include(g => g.Evidencias)
        .Include(g => g.CategoriaGasto);

    private static GastoDto ToDto(Gasto gasto) => new()
    {
        Id = gasto.Id,
        LocalId = gasto.LocalId,
        Descripcion = gasto.Descripcion,
        CategoriaGastoId = gasto.CategoriaGastoId,
        CategoriaGastoNombre = gasto.CategoriaGasto.Nombre,
        Monto = gasto.Monto,
        Fecha = gasto.Fecha,
        EmpleadoNombre = gasto.Empleado.Nombre,
        TieneEvidencia = gasto.Evidencias.Count > 0,
        EvidenciaUrls = gasto.Evidencias.Select(e => $"/api/gastos/evidencias/{e.Id}").ToList(),
        CierreDiarioId = gasto.CierreDiarioId
    };
}
