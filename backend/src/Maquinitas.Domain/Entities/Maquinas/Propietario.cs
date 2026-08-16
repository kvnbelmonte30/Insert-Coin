using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Maquinas;

public class Propietario : EntityBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<Maquina> Maquinas { get; set; } = new List<Maquina>();
}
