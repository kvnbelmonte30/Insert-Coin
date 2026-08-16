using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;
using Maquinitas.Domain.Entities.Semanas;

namespace Maquinitas.Domain.Entities.Cierres;

public class CierreSemanal : EntityBase
{
    public Guid SemanaId { get; set; }
    public Semana Semana { get; set; } = null!;

    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public decimal TotalReportado { get; set; }
    public decimal TotalEsperado { get; set; }
    public decimal Diferencia => TotalReportado - TotalEsperado;
    public EstadoCierre EstadoDiferencia { get; set; } = EstadoCierre.Correcto;

    /// <summary>Pendiente = registrado por el empleado, aún puede reabrirse. Confirmado = bloqueado por el administrador (sección 26-27).</summary>
    public bool Confirmado { get; set; } = false;

    public Guid CreadoPorUsuarioId { get; set; }
    public ApplicationUser CreadoPorUsuario { get; set; } = null!;

    public Guid? ConfirmadoPorUsuarioId { get; set; }
    public ApplicationUser? ConfirmadoPorUsuario { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaConfirmacion { get; set; }

    public ICollection<CierreSemanalDetalle> Detalles { get; set; } = new List<CierreSemanalDetalle>();
}
