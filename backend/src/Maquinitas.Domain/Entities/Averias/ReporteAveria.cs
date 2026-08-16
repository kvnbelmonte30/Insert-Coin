using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;
using Maquinitas.Domain.Entities.Maquinas;

namespace Maquinitas.Domain.Entities.Averias;

public class ReporteAveria : EntityBase
{
    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public Guid MaquinaId { get; set; }
    public Maquina Maquina { get; set; } = null!;

    public Guid EmpleadoId { get; set; }
    public ApplicationUser Empleado { get; set; } = null!;

    public string Problema { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public EstadoMaquina EstadoAlReportar { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public ICollection<EvidenciaAveria> Evidencias { get; set; } = new List<EvidenciaAveria>();
}
