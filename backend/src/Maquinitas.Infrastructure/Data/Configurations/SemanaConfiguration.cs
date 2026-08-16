using Maquinitas.Domain.Entities.Semanas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class SemanaConfiguration : IEntityTypeConfiguration<Semana>
{
    public void Configure(EntityTypeBuilder<Semana> builder)
    {
        builder.HasIndex(s => new { s.LocalId, s.Numero }).IsUnique();

        builder.HasOne(s => s.Local)
            .WithMany(l => l.Semanas)
            .HasForeignKey(s => s.LocalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
