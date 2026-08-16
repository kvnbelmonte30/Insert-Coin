using Maquinitas.Api.Dtos.Dashboard;
using Maquinitas.Domain.Common;
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
        var locales = await _db.Locales.CountAsync(l => l.Activo);
        var maquinas = await _db.Maquinas.CountAsync(m => m.Activo);

        var empleadosRoleId = await _db.Roles.Where(r => r.Name == Roles.Empleado).Select(r => r.Id).FirstOrDefaultAsync();
        var empleados = empleadosRoleId == Guid.Empty
            ? 0
            : await _db.UserRoles.CountAsync(ur => ur.RoleId == empleadosRoleId);

        var cierresCorrectos = await _db.CierresDiarios.CountAsync(c => c.Estado == EstadoCierre.Correcto);
        var cierresConDiferencia = await _db.CierresDiarios.CountAsync(c => c.Estado == EstadoCierre.Revisar);

        var maquinasAveriadas = await _db.Maquinas.CountAsync(m => m.Estado == EstadoMaquina.Reportada);
        var maquinasEnReparacion = await _db.Maquinas.CountAsync(m => m.Estado == EstadoMaquina.EnReparacion);

        var gastosPendientes = await _db.Gastos.CountAsync(g => !g.Evidencias.Any());

        var cuentasActivas = await _db.Cuentas
            .Include(c => c.Local)
            .Include(c => c.Semana)
            .Include(c => c.Detalles)
            .Where(c => c.Activa)
            .ToListAsync();

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
}
