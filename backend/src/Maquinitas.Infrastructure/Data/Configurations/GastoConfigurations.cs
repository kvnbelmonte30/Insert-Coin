using Maquinitas.Domain.Entities.Gastos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class GastoConfiguration : IEntityTypeConfiguration<Gasto>
{
    public void Configure(EntityTypeBuilder<Gasto> builder)
    {
        builder.Property(g => g.Descripcion).IsRequired().HasMaxLength(300);

        builder.HasOne(g => g.Local)
            .WithMany()
            .HasForeignKey(g => g.LocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.CierreDiario)
            .WithMany(c => c.Gastos)
            .HasForeignKey(g => g.CierreDiarioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(g => g.Empleado)
            .WithMany()
            .HasForeignKey(g => g.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EvidenciaConfiguration : IEntityTypeConfiguration<Evidencia>
{
    public void Configure(EntityTypeBuilder<Evidencia> builder)
    {
        builder.HasOne(e => e.Gasto)
            .WithMany(g => g.Evidencias)
            .HasForeignKey(e => e.GastoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
