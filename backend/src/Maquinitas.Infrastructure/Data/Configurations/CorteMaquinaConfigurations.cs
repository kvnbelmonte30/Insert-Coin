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
