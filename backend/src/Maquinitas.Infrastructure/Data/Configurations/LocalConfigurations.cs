using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class LocalConfiguration : IEntityTypeConfiguration<Local>
{
    public void Configure(EntityTypeBuilder<Local> builder)
    {
        builder.Property(l => l.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(l => l.Direccion).HasMaxLength(300);
    }
}

public class UsuarioLocalConfiguration : IEntityTypeConfiguration<UsuarioLocal>
{
    public void Configure(EntityTypeBuilder<UsuarioLocal> builder)
    {
        builder.HasIndex(ul => new { ul.UsuarioId, ul.LocalId }).IsUnique();

        builder.HasOne(ul => ul.Usuario)
            .WithMany(u => u.UsuarioLocales)
            .HasForeignKey(ul => ul.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ul => ul.Local)
            .WithMany(l => l.UsuarioLocales)
            .HasForeignKey(ul => ul.LocalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
