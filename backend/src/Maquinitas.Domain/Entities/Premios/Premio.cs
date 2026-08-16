using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Premios;

public class Premio : EntityBase
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Denominacion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<MaquinaPremio> ConfiguracionesMaquina { get; set; } = new List<MaquinaPremio>();
}
