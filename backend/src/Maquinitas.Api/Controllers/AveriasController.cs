using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Averias;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Averias;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Maquinas;
using Maquinitas.Domain.Entities.Notificaciones;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AveriasController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly FileStorageService _fileStorage;
    private readonly AuditoriaService _auditoria;
    private readonly UserManager<ApplicationUser> _userManager;

    public AveriasController(ApplicationDbContext db, FileStorageService fileStorage, AuditoriaService auditoria, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _fileStorage = fileStorage;
        _auditoria = auditoria;
        _userManager = userManager;
    }

    [HttpGet("local/{localId:guid}")]
    public async Task<ActionResult<IEnumerable<ReporteAveriaDto>>> GetByLocal(
        Guid localId,
        [FromQuery] DateOnly? desde = null,
        [FromQuery] DateOnly? hasta = null)
    {
        if (!CurrentUser.HasAccessToLocal(User, localId)) return Forbid();

        var query = BaseQuery().Where(r => r.LocalId == localId);
        if (desde is not null) query = query.Where(r => r.Fecha >= desde.Value.ToDateTime(TimeOnly.MinValue));
        if (hasta is not null) query = query.Where(r => r.Fecha < hasta.Value.AddDays(1).ToDateTime(TimeOnly.MinValue));

        var reportes = await query.OrderByDescending(r => r.Fecha).ToListAsync();
        return Ok(reportes.Select(ToDto));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Empleado)]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<ReporteAveriaDto>> Crear([FromForm] CrearReporteAveriaRequest request, [FromForm] List<IFormFile> evidencias)
    {
        if (!CurrentUser.HasAccessToLocal(User, request.LocalId)) return Forbid();

        var maquina = await _db.Maquinas.FirstOrDefaultAsync(m => m.Id == request.MaquinaId && m.LocalId == request.LocalId);
        if (maquina is null) return NotFound(new { message = "Máquina no encontrada en este local." });

        var reporte = new ReporteAveria
        {
            LocalId = request.LocalId,
            MaquinaId = request.MaquinaId,
            EmpleadoId = CurrentUser.GetId(User),
            Problema = request.Problema,
            Descripcion = request.Descripcion,
            EstadoAlReportar = maquina.Estado
        };

        foreach (var archivo in evidencias.Where(a => a.Length > 0))
        {
            var ruta = await _fileStorage.GuardarAsync($"averias/{reporte.Id}", archivo);
            reporte.Evidencias.Add(new EvidenciaAveria { RutaArchivo = ruta, NombreArchivo = archivo.FileName });
        }

        _db.ReportesAveria.Add(reporte);

        var estadoAnterior = maquina.Estado;
        maquina.Estado = EstadoMaquina.Reportada;
        _db.HistorialEstadosMaquina.Add(new HistorialEstadoMaquina
        {
            MaquinaId = maquina.Id,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = EstadoMaquina.Reportada,
            UsuarioId = CurrentUser.GetId(User),
            Comentario = $"Reporte de avería: {request.Problema}"
        });

        var admins = await _userManager.GetUsersInRoleAsync(Roles.Administrador);
        var localNombre = await _db.Locales.Where(l => l.Id == request.LocalId).Select(l => l.Nombre).FirstAsync();
        foreach (var admin in admins)
        {
            _db.Set<Notificacion>().Add(new Notificacion
            {
                UsuarioId = admin.Id,
                LocalId = request.LocalId,
                Tipo = TipoNotificacion.MaquinaReportada,
                Mensaje = $"{maquina.Nombre} en {localNombre} fue reportada: {request.Problema}.",
                ReferenciaId = reporte.Id
            });
        }

        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(CurrentUser.GetId(User), CurrentUser.GetNombre(User), request.LocalId,
            "Máquina reportada", nameof(ReporteAveria), reporte.Id.ToString(), null, new { request.Problema });

        var completo = await BaseQuery().FirstAsync(r => r.Id == reporte.Id);
        return Ok(ToDto(completo));
    }

    [HttpGet("evidencias/{evidenciaId:guid}")]
    public async Task<IActionResult> GetEvidencia(Guid evidenciaId)
    {
        var evidencia = await _db.EvidenciasAveria.Include(e => e.ReporteAveria).FirstOrDefaultAsync(e => e.Id == evidenciaId);
        if (evidencia is null) return NotFound();
        if (!CurrentUser.HasAccessToLocal(User, evidencia.ReporteAveria.LocalId)) return Forbid();

        var (stream, contentType) = _fileStorage.Abrir(evidencia.RutaArchivo);
        return File(stream, contentType);
    }

    private IQueryable<ReporteAveria> BaseQuery() => _db.ReportesAveria
        .Include(r => r.Maquina)
        .Include(r => r.Empleado)
        .Include(r => r.Evidencias);

    private static ReporteAveriaDto ToDto(ReporteAveria r) => new()
    {
        Id = r.Id,
        LocalId = r.LocalId,
        MaquinaId = r.MaquinaId,
        MaquinaNombre = r.Maquina.Nombre,
        EmpleadoNombre = r.Empleado.Nombre,
        Problema = r.Problema,
        Descripcion = r.Descripcion,
        EstadoAlReportar = r.EstadoAlReportar,
        Fecha = r.Fecha,
        EvidenciaUrls = r.Evidencias.Select(e => $"/api/averias/evidencias/{e.Id}").ToList()
    };
}
