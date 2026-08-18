using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cuentas;
using Maquinitas.Domain.Entities.Gastos;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Maquinas;
using Maquinitas.Domain.Entities.Premios;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Maquinitas.Infrastructure.Seed;

/// <summary>
/// Datos iniciales de arranque. Los valores (denominaciones, premios) quedan en la base de datos,
/// editables desde el panel de administración -- nunca quedan fijos en el código (sección 6-8).
/// </summary>
public static class DbSeeder
{
    /// <summary>Fijo para que la migración de datos (Tipo enum -> CategoriaGastoId) pueda referenciarlo de forma determinista.</summary>
    public static readonly Guid CategoriaGastoGeneralId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid CategoriaGastoReposicionId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public static readonly Guid CategoriaGastoSueldosId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    public static readonly Guid CategoriaGastoDepositoId = Guid.Parse("00000000-0000-0000-0000-000000000004");

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in new[] { Roles.Administrador, Roles.Empleado })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        if (!await userManager.Users.AnyAsync())
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@maquinitas.local",
                EmailConfirmed = true,
                Nombre = "Administrador",
                Activo = true
            };

            var result = await userManager.CreateAsync(admin, "Admin#2026!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Administrador);
            }
        }

        if (!await db.TiposMaquina.AnyAsync())
        {
            db.TiposMaquina.AddRange(
                new TipoMaquina { Nombre = "Ruleta" },
                new TipoMaquina { Nombre = "Pimbolera" },
                new TipoMaquina { Nombre = "Cascada" });
        }

        if (!await db.Denominaciones.AnyAsync())
        {
            db.Denominaciones.AddRange(
                new Denominacion { Tipo = TipoDenominacion.Bolsa, Valor = 10, ValorPorBolsa = 2000 },
                new Denominacion { Tipo = TipoDenominacion.Bolsa, Valor = 5, ValorPorBolsa = 2000 },
                new Denominacion { Tipo = TipoDenominacion.Bolsa, Valor = 1, ValorPorBolsa = 1000 },

                new Denominacion { Tipo = TipoDenominacion.Moneda, Valor = 0.50m },
                new Denominacion { Tipo = TipoDenominacion.Moneda, Valor = 1 },
                new Denominacion { Tipo = TipoDenominacion.Moneda, Valor = 2 },
                new Denominacion { Tipo = TipoDenominacion.Moneda, Valor = 5 },
                new Denominacion { Tipo = TipoDenominacion.Moneda, Valor = 10 },
                new Denominacion { Tipo = TipoDenominacion.Moneda, Valor = 20 },

                new Denominacion { Tipo = TipoDenominacion.Billete, Valor = 20 },
                new Denominacion { Tipo = TipoDenominacion.Billete, Valor = 50 },
                new Denominacion { Tipo = TipoDenominacion.Billete, Valor = 100 },
                new Denominacion { Tipo = TipoDenominacion.Billete, Valor = 200 },
                new Denominacion { Tipo = TipoDenominacion.Billete, Valor = 500 },
                new Denominacion { Tipo = TipoDenominacion.Billete, Valor = 1000 }
            );
        }

        if (!await db.Premios.AnyAsync())
        {
            decimal[] valoresPremio = { 1300, 650, 530, 230, 130, 70, 40 };
            db.Premios.AddRange(valoresPremio.Select(v => new Premio
            {
                Nombre = $"${v:0.##}",
                Denominacion = v
            }));
        }

        if (!await db.CategoriasGasto.AnyAsync())
        {
            db.CategoriasGasto.AddRange(
                new CategoriaGasto { Id = CategoriaGastoGeneralId, Nombre = "Gasto general" },
                new CategoriaGasto { Id = CategoriaGastoReposicionId, Nombre = "Reposición de fondo de máquina" },
                new CategoriaGasto { Id = CategoriaGastoSueldosId, Nombre = "Sueldos de empleados" },
                new CategoriaGasto { Id = CategoriaGastoDepositoId, Nombre = "Depósito a administrador" });
        }

        await db.SaveChangesAsync();
    }
}
