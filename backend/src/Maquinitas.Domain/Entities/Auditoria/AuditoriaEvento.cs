using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Auditoria;

/// <summary>Registro genérico de auditoría (sección 29): usuario, fecha, local, acción, registro afectado, valor anterior/nuevo.</summary>
public class AuditoriaEvento : EntityBase
{
    public Guid UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;

    public Guid? LocalId { get; set; }

    public string Accion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public string EntidadId { get; set; } = string.Empty;

    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
