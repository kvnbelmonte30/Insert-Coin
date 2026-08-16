using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Maquinas;

namespace Maquinitas.Domain.Entities.Premios;

/// <summary>Configuración de cuántos premios de cada denominación debe tener una cascada (ConfiguracionPremios + MaquinaPremios del plan).</summary>
public class MaquinaPremio : EntityBase
{
    public Guid MaquinaId { get; set; }
    public Maquina Maquina { get; set; } = null!;

    public Guid PremioId { get; set; }
    public Premio Premio { get; set; } = null!;

    public int CantidadAsignada { get; set; }
    public bool Activo { get; set; } = true;
}
