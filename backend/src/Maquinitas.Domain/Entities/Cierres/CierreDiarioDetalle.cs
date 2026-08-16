using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cuentas;
using Maquinitas.Domain.Entities.Premios;

namespace Maquinitas.Domain.Entities.Cierres;

public class CierreDiarioDetalle : EntityBase
{
    public Guid CierreDiarioId { get; set; }
    public CierreDiario CierreDiario { get; set; } = null!;

    public ConceptoMovimiento Concepto { get; set; }

    public Guid? DenominacionId { get; set; }
    public Denominacion? Denominacion { get; set; }

    public Guid? PremioId { get; set; }
    public Premio? Premio { get; set; }

    /// <summary>Cantidad de piezas/bolsas contadas. Para Terminal/Transferencia siempre es 1.</summary>
    public int Cantidad { get; set; }

    /// <summary>Valor unitario (denominación) o el monto directo capturado (Terminal/Transferencia).</summary>
    public decimal ValorUnitario { get; set; }

    public decimal Subtotal => Cantidad * ValorUnitario;
}
