using Maquinitas.Domain.Entities.Auditoria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maquinitas.Infrastructure.Data.Configurations;

public class AuditoriaEventoConfiguration : IEntityTypeConfiguration<AuditoriaEvento>
{
    public void Configure(EntityTypeBuilder<AuditoriaEvento> builder)
    {
        builder.Property(a => a.Accion).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Entidad).IsRequired().HasMaxLength(100);
        builder.HasIndex(a => new { a.Entidad, a.EntidadId });
        builder.HasIndex(a => a.Fecha);
    }
}
