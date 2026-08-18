using Maquinitas.Domain.Entities.Auditoria;
using Maquinitas.Domain.Entities.Averias;
using Maquinitas.Domain.Entities.Cierres;
using Maquinitas.Domain.Entities.Cuentas;
using Maquinitas.Domain.Entities.Gastos;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;
using Maquinitas.Domain.Entities.Maquinas;
using Maquinitas.Domain.Entities.Notificaciones;
using Maquinitas.Domain.Entities.Premios;
using Maquinitas.Domain.Entities.Semanas;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UsuarioLocal> UsuarioLocales => Set<UsuarioLocal>();
    public DbSet<Local> Locales => Set<Local>();

    public DbSet<Propietario> Propietarios => Set<Propietario>();
    public DbSet<TipoMaquina> TiposMaquina => Set<TipoMaquina>();
    public DbSet<Maquina> Maquinas => Set<Maquina>();
    public DbSet<HistorialEstadoMaquina> HistorialEstadosMaquina => Set<HistorialEstadoMaquina>();

    public DbSet<Premio> Premios => Set<Premio>();
    public DbSet<MaquinaPremio> MaquinaPremios => Set<MaquinaPremio>();
    public DbSet<InventarioPremio> InventariosPremios => Set<InventarioPremio>();
    public DbSet<InventarioPremioDetalle> InventarioPremioDetalles => Set<InventarioPremioDetalle>();

    public DbSet<Denominacion> Denominaciones => Set<Denominacion>();
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();
    public DbSet<CuentaDetalle> CuentaDetalles => Set<CuentaDetalle>();
    public DbSet<CuentaModificacionLog> CuentaModificacionLogs => Set<CuentaModificacionLog>();

    public DbSet<Semana> Semanas => Set<Semana>();

    public DbSet<CierreDiario> CierresDiarios => Set<CierreDiario>();
    public DbSet<CierreDiarioDetalle> CierreDiarioDetalles => Set<CierreDiarioDetalle>();
    public DbSet<CierreSemanal> CierresSemanales => Set<CierreSemanal>();
    public DbSet<CierreSemanalDetalle> CierreSemanalDetalles => Set<CierreSemanalDetalle>();

    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<CategoriaGasto> CategoriasGasto => Set<CategoriaGasto>();
    public DbSet<Evidencia> Evidencias => Set<Evidencia>();

    public DbSet<ReporteAveria> ReportesAveria => Set<ReporteAveria>();
    public DbSet<EvidenciaAveria> EvidenciasAveria => Set<EvidenciaAveria>();

    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<AuditoriaEvento> AuditoriaEventos => Set<AuditoriaEvento>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }
}
