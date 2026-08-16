using Maquinitas.Domain.Entities.Notificaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.Property(n => n.Mensaje).IsRequired().HasMaxLength(500);
        builder.HasIndex(n => new { n.UsuarioId, n.Leida });

        builder.HasOne(n => n.Usuario)
            .WithMany()
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Local)
            .WithMany()
            .HasForeignKey(n => n.LocalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
