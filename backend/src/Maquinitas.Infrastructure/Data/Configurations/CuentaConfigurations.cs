using Maquinitas.Domain.Entities.Cuentas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class DenominacionConfiguration : IEntityTypeConfiguration<Denominacion>
{
    public void Configure(EntityTypeBuilder<Denominacion> builder)
    {
    }
}

public class CuentaConfiguration : IEntityTypeConfiguration<Cuenta>
{
    public void Configure(EntityTypeBuilder<Cuenta> builder)
    {
        builder.HasOne(c => c.Local)
            .WithMany(l => l.Cuentas)
            .HasForeignKey(c => c.LocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Semana)
            .WithMany()
            .HasForeignKey(c => c.SemanaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CreadoPorUsuario)
            .WithMany()
            .HasForeignKey(c => c.CreadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CuentaDetalleConfiguration : IEntityTypeConfiguration<CuentaDetalle>
{
    public void Configure(EntityTypeBuilder<CuentaDetalle> builder)
    {
        builder.Ignore(d => d.Subtotal);

        builder.HasOne(d => d.Cuenta)
            .WithMany(c => c.Detalles)
            .HasForeignKey(d => d.CuentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Denominacion)
            .WithMany()
            .HasForeignKey(d => d.DenominacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Premio)
            .WithMany()
            .HasForeignKey(d => d.PremioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.RegistradoPorUsuario)
            .WithMany()
            .HasForeignKey(d => d.RegistradoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CuentaModificacionLogConfiguration : IEntityTypeConfiguration<CuentaModificacionLog>
{
    public void Configure(EntityTypeBuilder<CuentaModificacionLog> builder)
    {
        builder.HasOne(m => m.Cuenta)
            .WithMany(c => c.Modificaciones)
            .HasForeignKey(m => m.CuentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Usuario)
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
