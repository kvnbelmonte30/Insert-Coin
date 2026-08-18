using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cierres;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;

namespace Maquinitas.Domain.Entities.Gastos;

public class Gasto : EntityBase
{
    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public Guid? CierreDiarioId { get; set; }
    public CierreDiario? CierreDiario { get; set; }

    public Guid EmpleadoId { get; set; }
    public ApplicationUser Empleado { get; set; } = null!;

    public string Descripcion { get; set; } = string.Empty;

    public Guid CategoriaGastoId { get; set; }
    public CategoriaGasto CategoriaGasto { get; set; } = null!;

    public decimal Monto { get; set; }
    public DateOnly Fecha { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
}
