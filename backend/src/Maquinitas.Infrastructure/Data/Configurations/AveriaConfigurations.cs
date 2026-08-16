using Maquinitas.Domain.Entities.Averias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class ReporteAveriaConfiguration : IEntityTypeConfiguration<ReporteAveria>
{
    public void Configure(EntityTypeBuilder<ReporteAveria> builder)
    {
        builder.Property(r => r.Problema).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Descripcion).HasMaxLength(1000);

        builder.HasOne(r => r.Local)
            .WithMany()
            .HasForeignKey(r => r.LocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Maquina)
            .WithMany(m => m.ReportesAveria)
            .HasForeignKey(r => r.MaquinaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Empleado)
            .WithMany()
            .HasForeignKey(r => r.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EvidenciaAveriaConfiguration : IEntityTypeConfiguration<EvidenciaAveria>
{
    public void Configure(EntityTypeBuilder<EvidenciaAveria> builder)
    {
        builder.HasOne(e => e.ReporteAveria)
            .WithMany(r => r.Evidencias)
            .HasForeignKey(e => e.ReporteAveriaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
