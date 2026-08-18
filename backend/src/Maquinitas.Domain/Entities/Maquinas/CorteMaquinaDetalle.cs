using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cuentas;
using Maquinitas.Domain.Entities.Premios;

namespace Maquinitas.Domain.Entities.Maquinas;

public class CorteMaquinaDetalle : EntityBase
{
    public Guid CorteMaquinaId { get; set; }
    public CorteMaquina CorteMaquina { get; set; } = null!;

    public Guid? DenominacionId { get; set; }
    public Denominacion? Denominacion { get; set; }

    public Guid? PremioId { get; set; }
    public Premio? Premio { get; set; }

    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Subtotal => Cantidad * ValorUnitario;
}
