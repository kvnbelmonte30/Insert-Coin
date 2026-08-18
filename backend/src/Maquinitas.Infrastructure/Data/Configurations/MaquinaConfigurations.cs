using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Maquinas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class PropietarioConfiguration : IEntityTypeConfiguration<Propietario>
{
    public void Configure(EntityTypeBuilder<Propietario> builder)
    {
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
    }
}

public class ApplicationUserPropietarioConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(u => u.Propietario)
            .WithMany()
            .HasForeignKey(u => u.PropietarioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class TipoMaquinaConfiguration : IEntityTypeConfiguration<TipoMaquina>
{
    public void Configure(EntityTypeBuilder<TipoMaquina> builder)
    {
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(80);
        builder.HasIndex(t => t.Nombre).IsUnique();
    }
}

public class MaquinaConfiguration : IEntityTypeConfiguration<Maquina>
{
    public void Configure(EntityTypeBuilder<Maquina> builder)
    {
        builder.Property(m => m.Nombre).IsRequired().HasMaxLength(150);

        builder.HasOne(m => m.TipoMaquina)
            .WithMany(t => t.Maquinas)
            .HasForeignKey(m => m.TipoMaquinaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Local)
            .WithMany(l => l.Maquinas)
            .HasForeignKey(m => m.LocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Propietario)
            .WithMany(p => p.Maquinas)
            .HasForeignKey(m => m.PropietarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class HistorialEstadoMaquinaConfiguration : IEntityTypeConfiguration<HistorialEstadoMaquina>
{
    public void Configure(EntityTypeBuilder<HistorialEstadoMaquina> builder)
    {
        builder.HasOne(h => h.Maquina)
            .WithMany(m => m.HistorialEstados)
            .HasForeignKey(h => h.MaquinaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.Usuario)
            .WithMany()
            .HasForeignKey(h => h.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
