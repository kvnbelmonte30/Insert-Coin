using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;

namespace Maquinitas.Domain.Entities.Notificaciones;

public class Notificacion : EntityBase
{
    public Guid UsuarioId { get; set; }
    public ApplicationUser Usuario { get; set; } = null!;

    public Guid? LocalId { get; set; }
    public Local? Local { get; set; }

    public TipoNotificacion Tipo { get; set; }
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>Id del registro relacionado (p. ej. CuentaId) para poder navegar a "Ver cambios".</summary>
    public Guid? ReferenciaId { get; set; }

    public bool Leida { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
