using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Maquinas;

public class TipoMaquina : EntityBase
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<Maquina> Maquinas { get; set; } = new List<Maquina>();
}
