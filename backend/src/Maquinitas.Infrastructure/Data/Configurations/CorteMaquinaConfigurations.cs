using Maquinitas.Domain.Entities.Maquinas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class CorteMaquinaConfiguration : IEntityTypeConfiguration<CorteMaquina>
{
    public void Configure(EntityTypeBuilder<CorteMaquina> builder)
    {
        builder.HasIndex(c => new { c.MaquinaId, c.Fecha });

        builder.HasOne(c => c.Maquina)
            .WithMany()
            .HasForeignKey(c => c.MaquinaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Local)
            .WithMany()
            .HasForeignKey(c => c.LocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.RegistradoPor)
            .WithMany()
            .HasForeignKey(c => c.RegistradoPorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CorteMaquinaDetalleConfiguration : IEntityTypeConfiguration<CorteMaquinaDetalle>
{
    public void Configure(EntityTypeBuilder<CorteMaquinaDetalle> builder)
    {
        builder.Ignore(d => d.Subtotal);

        builder.HasOne(d => d.CorteMaquina)
            .WithMany(c => c.Detalles)
            .HasForeignKey(d => d.CorteMaquinaId)
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
