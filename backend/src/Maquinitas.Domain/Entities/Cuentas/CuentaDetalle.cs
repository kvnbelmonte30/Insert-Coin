using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Premios;

namespace Maquinitas.Domain.Entities.Cuentas;

/// <summary>
/// Una línea de la cuenta (una bolsa, una moneda o un premio disponible). Cada línea conserva su
/// propia cantidad, denominación, subtotal, origen, fecha y usuario (sección 35, "la cuenta siempre suma").
/// </summary>
public class CuentaDetalle : EntityBase
{
    public Guid CuentaId { get; set; }
    public Cuenta Cuenta { get; set; } = null!;

    public Guid? DenominacionId { get; set; }
    public Denominacion? Denominacion { get; set; }

    public Guid? PremioId { get; set; }
    public Premio? Premio { get; set; }

    public int Cantidad { get; set; }

    /// <summary>Snapshot del valor unitario al momento del registro (bolsa, moneda, billete o premio).</summary>
    public decimal ValorUnitario { get; set; }

    public decimal Subtotal => Cantidad * ValorUnitario;

    public string Origen { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public Guid RegistradoPorUsuarioId { get; set; }
    public ApplicationUser RegistradoPorUsuario { get; set; } = null!;
}
