using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cuentas;
using Maquinitas.Domain.Entities.Premios;

namespace Maquinitas.Domain.Entities.Cierres;

/// <summary>Igual que CierreDiarioDetalle, pero además incluye "Premios puestos" durante la semana (sección 25).</summary>
public class CierreSemanalDetalle : EntityBase
{
    public Guid CierreSemanalId { get; set; }
    public CierreSemanal CierreSemanal { get; set; } = null!;

    public ConceptoMovimiento Concepto { get; set; }

    public Guid? DenominacionId { get; set; }
    public Denominacion? Denominacion { get; set; }

    public Guid? PremioId { get; set; }
    public Premio? Premio { get; set; }

    public bool EsPremioPuesto { get; set; } = false;

    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Subtotal => Cantidad * ValorUnitario;
}
