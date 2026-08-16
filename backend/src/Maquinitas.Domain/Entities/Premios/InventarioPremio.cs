using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Maquinas;

namespace Maquinitas.Domain.Entities.Premios;

/// <summary>Encabezado de un conteo periódico de premios en una cascada (sección 22-23 del plan).</summary>
public class InventarioPremio : EntityBase
{
    public Guid MaquinaId { get; set; }
    public Maquina Maquina { get; set; } = null!;

    public Guid UsuarioId { get; set; }
    public ApplicationUser Usuario { get; set; } = null!;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public ICollection<InventarioPremioDetalle> Detalles { get; set; } = new List<InventarioPremioDetalle>();
}
