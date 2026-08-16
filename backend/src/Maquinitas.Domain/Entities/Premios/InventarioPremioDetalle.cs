using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Premios;

public class InventarioPremioDetalle : EntityBase
{
    public Guid InventarioPremioId { get; set; }
    public InventarioPremio InventarioPremio { get; set; } = null!;

    public Guid PremioId { get; set; }
    public Premio Premio { get; set; } = null!;

    /// <summary>Snapshot de la cantidad configurada al momento del conteo, para preservar el historial aunque la configuración cambie después.</summary>
    public int CantidadConfigurada { get; set; }
    public int CantidadEncontrada { get; set; }
    public int Diferencia => CantidadConfigurada - CantidadEncontrada;
}
