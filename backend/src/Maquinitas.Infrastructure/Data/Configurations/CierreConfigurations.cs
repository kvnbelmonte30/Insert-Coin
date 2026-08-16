using Maquinitas.Domain.Entities.Cierres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class CierreDiarioConfiguration : IEntityTypeConfiguration<CierreDiario>
{
    public void Configure(EntityTypeBuilder<CierreDiario> builder)
    {
        builder.Ignore(c => c.Diferencia);
        builder.HasIndex(c => new { c.LocalId, c.EmpleadoId, c.Fecha });

        builder.HasOne(c => c.Local)
            .WithMany()
            .HasForeignKey(c => c.LocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Semana)
            .WithMany()
            .HasForeignKey(c => c.SemanaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Empleado)
            .WithMany()
            .HasForeignKey(c => c.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CierreDiarioDetalleConfiguration : IEntityTypeConfiguration<CierreDiarioDetalle>
{
    public void Configure(EntityTypeBuilder<CierreDiarioDetalle> builder)
    {
        builder.Ignore(d => d.Subtotal);

        builder.HasOne(d => d.CierreDiario)
            .WithMany(c => c.Detalles)
            .HasForeignKey(d => d.CierreDiarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Denominacion)
            .WithMany()
            .HasForeignKey(d => d.DenominacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Premio)
            .WithMany()
            .HasForeignKey(d => d.PremioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CierreSemanalConfiguration : IEntityTypeConfiguration<CierreSemanal>
{
    public void Configure(EntityTypeBuilder<CierreSemanal> builder)
    {
        builder.Ignore(c => c.Diferencia);
        builder.HasIndex(c => c.SemanaId).IsUnique();

        builder.HasOne(c => c.Semana)
            .WithMany()
            .HasForeignKey(c => c.SemanaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Local)
            .WithMany()
            .HasForeignKey(c => c.LocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CreadoPorUsuario)
            .WithMany()
            .HasForeignKey(c => c.CreadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ConfirmadoPorUsuario)
            .WithMany()
            .HasForeignKey(c => c.ConfirmadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CierreSemanalDetalleConfiguration : IEntityTypeConfiguration<CierreSemanalDetalle>
{
    public void Configure(EntityTypeBuilder<CierreSemanalDetalle> builder)
    {
        builder.Ignore(d => d.Subtotal);

        builder.HasOne(d => d.CierreSemanal)
            .WithMany(c => c.Detalles)
            .HasForeignKey(d => d.CierreSemanalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Denominacion)
            .WithMany()
            .HasForeignKey(d => d.DenominacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Premio)
            .WithMany()
            .HasForeignKey(d => d.PremioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
