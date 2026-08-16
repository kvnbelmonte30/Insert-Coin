using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Averias;
using Maquinitas.Domain.Entities.Locales;
using Maquinitas.Domain.Entities.Premios;

namespace Maquinitas.Domain.Entities.Maquinas;

public class Maquina : EntityBase
{
    public string Nombre { get; set; } = string.Empty;

    public Guid TipoMaquinaId { get; set; }
    public TipoMaquina TipoMaquina { get; set; } = null!;

    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public Guid PropietarioId { get; set; }
    public Propietario Propietario { get; set; } = null!;

    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public EstadoMaquina Estado { get; set; } = EstadoMaquina.Funcional;
    public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    public ICollection<HistorialEstadoMaquina> HistorialEstados { get; set; } = new List<HistorialEstadoMaquina>();
    public ICollection<MaquinaPremio> ConfiguracionPremios { get; set; } = new List<MaquinaPremio>();
    public ICollection<InventarioPremio> InventariosPremios { get; set; } = new List<InventarioPremio>();
    public ICollection<ReporteAveria> ReportesAveria { get; set; } = new List<ReporteAveria>();
}
