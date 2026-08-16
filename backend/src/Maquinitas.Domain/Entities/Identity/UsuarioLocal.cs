using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Locales;

namespace Maquinitas.Domain.Entities.Identity;

public class UsuarioLocal : EntityBase
{
    public Guid UsuarioId { get; set; }
    public ApplicationUser Usuario { get; set; } = null!;

    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
}
