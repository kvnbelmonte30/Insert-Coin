using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Gastos;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;
using Maquinitas.Domain.Entities.Semanas;

namespace Maquinitas.Domain.Entities.Cierres;

/// <summary>
/// Cierre diario del empleado. Los totales se calculan sumando los conceptos registrados
/// (sección 14): nunca "Total = Cuenta - Gastos - Premios".
/// </summary>
public class CierreDiario : EntityBase
{
    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public Guid SemanaId { get; set; }
    public Semana Semana { get; set; } = null!;

    public Guid EmpleadoId { get; set; }
    public ApplicationUser Empleado { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    /// <summary>Suma de todos los conceptos reportados por el empleado (efectivo, terminal, transferencias, premios, gastos).</summary>
    public decimal TotalReportado { get; set; }

    /// <summary>Suma esperada según la cuenta configurada por el administrador para la semana.</summary>
    public decimal TotalEsperado { get; set; }

    public decimal Diferencia => TotalReportado - TotalEsperado;
    public EstadoCierre Estado { get; set; } = EstadoCierre.Correcto;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public bool Editable { get; set; } = true;

    public ICollection<CierreDiarioDetalle> Detalles { get; set; } = new List<CierreDiarioDetalle>();
    public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
}
