using Maquinitas.Domain.Entities.Premios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class PremioConfiguration : IEntityTypeConfiguration<Premio>
{
    public void Configure(EntityTypeBuilder<Premio> builder)
    {
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(80);
    }
}

public class MaquinaPremioConfiguration : IEntityTypeConfiguration<MaquinaPremio>
{
    public void Configure(EntityTypeBuilder<MaquinaPremio> builder)
    {
        builder.HasIndex(mp => new { mp.MaquinaId, mp.PremioId }).IsUnique();

        builder.HasOne(mp => mp.Maquina)
            .WithMany(m => m.ConfiguracionPremios)
            .HasForeignKey(mp => mp.MaquinaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mp => mp.Premio)
            .WithMany(p => p.ConfiguracionesMaquina)
            .HasForeignKey(mp => mp.PremioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InventarioPremioConfiguration : IEntityTypeConfiguration<InventarioPremio>
{
    public void Configure(EntityTypeBuilder<InventarioPremio> builder)
    {
        builder.HasOne(i => i.Maquina)
            .WithMany(m => m.InventariosPremios)
            .HasForeignKey(i => i.MaquinaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Usuario)
            .WithMany()
            .HasForeignKey(i => i.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InventarioPremioDetalleConfiguration : IEntityTypeConfiguration<InventarioPremioDetalle>
{
    public void Configure(EntityTypeBuilder<InventarioPremioDetalle> builder)
    {
        builder.HasOne(d => d.InventarioPremio)
            .WithMany(i => i.Detalles)
            .HasForeignKey(d => d.InventarioPremioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Premio)
            .WithMany()
            .HasForeignKey(d => d.PremioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(d => d.Diferencia);
    }
}
