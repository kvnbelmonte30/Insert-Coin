using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Dashboard;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Averias;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Administrador)]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var visibleIds = CurrentUser.GetVisibleLocalIds(User);

        var localesQuery = _db.Locales.Where(l => l.Activo);
        var maquinasQuery = _db.Maquinas.Where(m => m.Activo);
        var cierresQuery = _db.CierresDiarios.AsQueryable();
        var gastosQuery = _db.Gastos.Where(g => !g.Evidencias.Any());
        var cuentasQuery = _db.Cuentas.Include(c => c.Local).Include(c => c.Semana).Include(c => c.Detalles).Where(c => c.Activa);

        if (visibleIds is not null)
        {
            localesQuery = localesQuery.Where(l => visibleIds.Contains(l.Id));
            maquinasQuery = maquinasQuery.Where(m => visibleIds.Contains(m.LocalId));
            cierresQuery = cierresQuery.Where(c => visibleIds.Contains(c.LocalId));
            gastosQuery = gastosQuery.Where(g => visibleIds.Contains(g.LocalId));
            cuentasQuery = cuentasQuery.Where(c => visibleIds.Contains(c.LocalId));
        }

        var locales = await localesQuery.CountAsync();
        var maquinas = await maquinasQuery.CountAsync();

        var empleadosRoleId = await _db.Roles.Where(r => r.Name == Roles.Empleado).Select(r => r.Id).FirstOrDefaultAsync();
        int empleados;
        if (empleadosRoleId == Guid.Empty)
        {
            empleados = 0;
        }
        else if (visibleIds is null)
        {
            empleados = await _db.UserRoles.CountAsync(ur => ur.RoleId == empleadosRoleId);
        }
        else
        {
            empleados = await _db.UserRoles
                .Where(ur => ur.RoleId == empleadosRoleId)
                .Join(_db.UsuarioLocales, ur => ur.UserId, ul => ul.UsuarioId, (ur, ul) => ul)
                .Where(ul => visibleIds.Contains(ul.LocalId))
                .Select(ul => ul.UsuarioId)
                .Distinct()
                .CountAsync();
        }

        var cierresCorrectos = await cierresQuery.CountAsync(c => c.Estado == EstadoCierre.Correcto);
        var cierresConDiferencia = await cierresQuery.CountAsync(c => c.Estado == EstadoCierre.Revisar);

        var maquinasAveriadas = await maquinasQuery.CountAsync(m => m.Estado == EstadoMaquina.Reportada);
        var maquinasEnReparacion = await maquinasQuery.CountAsync(m => m.Estado == EstadoMaquina.EnReparacion);

        var gastosPendientes = await gastosQuery.CountAsync();

        var cuentasActivas = await cuentasQuery.ToListAsync();

        var totalesPorLocal = cuentasActivas.Select(c => new TotalPorLocalDto
        {
            LocalId = c.LocalId,
            LocalNombre = c.Local.Nombre,
            SemanaNumero = c.Semana.Numero,
            TotalAcumulado = c.Detalles.Sum(d => d.Subtotal)
        }).ToList();

        return Ok(new DashboardDto
        {
            Locales = locales,
            Maquinas = maquinas,
            Empleados = empleados,
            CierresCorrectos = cierresCorrectos,
            CierresConDiferencia = cierresConDiferencia,
            MaquinasAveriadas = maquinasAveriadas,
            MaquinasEnReparacion = maquinasEnReparacion,
            GastosPendientesEvidencia = gastosPendientes,
            TotalAcumuladoSemanaActual = totalesPorLocal.Sum(t => t.TotalAcumulado),
            TotalesPorLocal = totalesPorLocal
        });
    }

    [HttpGet("propietarios")]
    public async Task<ActionResult<IEnumerable<PropietarioResumenDto>>> GetPropietarios()
    {
        var visibleIds = CurrentUser.GetVisibleLocalIds(User);

        var propietarios = await _db.Propietarios
            .Where(p => p.Activo)
            .Select(p => new PropietarioResumenDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                TotalMaquinas = p.Maquinas.Count(m => m.Activo && (visibleIds == null || visibleIds.Contains(m.LocalId)))
            })
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        if (visibleIds is not null)
        {
            propietarios = propietarios.Where(p => p.TotalMaquinas > 0).ToList();
        }

        return Ok(propietarios);
    }

    [HttpGet("mi-propietario")]
    public async Task<ActionResult<PropietarioDashboardDto>> GetMiPropietario()
    {
        var usuario = await _db.Users.FindAsync(CurrentUser.GetId(User));
        if (usuario?.PropietarioId is null)
        {
            return NotFound(new { message = "Tu usuario no tiene un propietario asociado. Pídele a otro administrador que lo configure en Usuarios." });
        }

        return await BuildPropietarioDashboard(usuario.PropietarioId.Value);
    }

    [HttpGet("propietario/{id:guid}")]
    public async Task<ActionResult<PropietarioDashboardDto>> GetPropietarioDashboard(Guid id)
    {
        return await BuildPropietarioDashboard(id);
    }

    private static readonly Dictionary<EstadoMaquina, string> EstadoLabels = new()
    {
        [EstadoMaquina.Funcional] = "Funcional",
        [EstadoMaquina.Reportada] = "Reportada",
        [EstadoMaquina.EnReparacion] = "En reparación",
        [EstadoMaquina.Reparada] = "Reparada",
        [EstadoMaquina.FueraDeServicio] = "Fuera de servicio"
    };

    private async Task<ActionResult<PropietarioDashboardDto>> BuildPropietarioDashboard(Guid propietarioId)
    {
        var propietario = await _db.Propietarios.FindAsync(propietarioId);
        if (propietario is null) return NotFound();

        var visibleIds = CurrentUser.GetVisibleLocalIds(User);
        var maquinasQuery = _db.Maquinas
            .Include(m => m.TipoMaquina)
            .Include(m => m.Local)
            .Where(m => m.PropietarioId == propietarioId && m.Activo);
        if (visibleIds is not null) maquinasQuery = maquinasQuery.Where(m => visibleIds.Contains(m.LocalId));

        var maquinas = await maquinasQuery.ToListAsync();

        var maquinaIds = maquinas.Select(m => m.Id).ToList();

        var ahora = DateTime.UtcNow;
        var desde30 = ahora.AddDays(-30);
        var desde90 = ahora.AddDays(-90);
        var desdeTendencia = InicioSemana(ahora.AddDays(-7 * 7));

        var desdeTendenciaUtc = DateTime.SpecifyKind(desdeTendencia.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var averias = maquinaIds.Count == 0
            ? new List<ReporteAveria>()
            : await _db.ReportesAveria
                .Where(r => maquinaIds.Contains(r.MaquinaId) && r.Fecha >= desdeTendenciaUtc)
                .ToListAsync();

        var averiasPorMaquina = averias
            .Where(a => a.Fecha >= desde30)
            .GroupBy(a => a.MaquinaId)
            .ToDictionary(g => g.Key, g => g.Count());

        var maquinasResumen = maquinas.Select(m => new MaquinaResumenDto
        {
            Id = m.Id,
            Nombre = m.Nombre,
            TipoNombre = m.TipoMaquina.Nombre,
            LocalNombre = m.Local.Nombre,
            Estado = EstadoLabels.GetValueOrDefault(m.Estado, m.Estado.ToString()),
            AveriasUltimos30Dias = averiasPorMaquina.GetValueOrDefault(m.Id, 0)
        }).OrderByDescending(m => m.AveriasUltimos30Dias).ThenBy(m => m.Nombre).ToList();

        var maquinasPorEstado = maquinas
            .GroupBy(m => m.Estado)
            .Select(g => new ConteoDto { Etiqueta = EstadoLabels.GetValueOrDefault(g.Key, g.Key.ToString()), Cantidad = g.Count() })
            .OrderByDescending(c => c.Cantidad)
            .ToList();

        var maquinasPorTipo = maquinas
            .GroupBy(m => m.TipoMaquina.Nombre)
            .Select(g => new ConteoDto { Etiqueta = g.Key, Cantidad = g.Count() })
            .OrderByDescending(c => c.Cantidad)
            .ToList();

        var tendencia = new List<AveriaTendenciaDto>();
        for (var semana = desdeTendencia; semana <= InicioSemana(ahora); semana = semana.AddDays(7))
        {
            var cantidad = averias.Count(a => InicioSemana(a.Fecha) == semana);
            tendencia.Add(new AveriaTendenciaDto
            {
                Periodo = $"{semana:dd/MM}",
                InicioSemana = semana,
                Cantidad = cantidad
            });
        }

        var tiempoPromedio = await CalcularTiempoPromedioReparacionAsync(maquinaIds);

        return Ok(new PropietarioDashboardDto
        {
            PropietarioId = propietario.Id,
            PropietarioNombre = propietario.Nombre,
            TotalMaquinas = maquinas.Count,
            MaquinasPorEstado = maquinasPorEstado,
            MaquinasPorTipo = maquinasPorTipo,
            Maquinas = maquinasResumen,
            AveriasUltimos30Dias = averias.Count(a => a.Fecha >= desde30),
            AveriasUltimos90Dias = averias.Count(a => a.Fecha >= desde90),
            AveriasPorSemana = tendencia,
            TiempoPromedioReparacionHoras = tiempoPromedio
        });
    }

    private async Task<double?> CalcularTiempoPromedioReparacionAsync(List<Guid> maquinaIds)
    {
        if (maquinaIds.Count == 0) return null;

        var historial = await _db.HistorialEstadosMaquina
            .Where(h => maquinaIds.Contains(h.MaquinaId))
            .OrderBy(h => h.Fecha)
            .ToListAsync();

        var duraciones = new List<double>();
        var reportadoEn = new Dictionary<Guid, DateTime>();

        foreach (var h in historial)
        {
            if (h.EstadoNuevo == EstadoMaquina.Reportada)
            {
                reportadoEn[h.MaquinaId] = h.Fecha;
            }
            else if ((h.EstadoNuevo == EstadoMaquina.Reparada || h.EstadoNuevo == EstadoMaquina.Funcional)
                     && reportadoEn.TryGetValue(h.MaquinaId, out var inicio))
            {
                duraciones.Add((h.Fecha - inicio).TotalHours);
                reportadoEn.Remove(h.MaquinaId);
            }
        }

        return duraciones.Count == 0 ? null : Math.Round(duraciones.Average(), 1);
    }

    private static DateOnly InicioSemana(DateTime fecha)
    {
        var date = DateOnly.FromDateTime(fecha);
        var diff = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-diff);
    }
}
