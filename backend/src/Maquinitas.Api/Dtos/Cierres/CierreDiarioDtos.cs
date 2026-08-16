using Maquinitas.Domain.Common;

namespace Maquinitas.Api.Dtos.Cierres;

public class LineaCierreRequest
{
    public ConceptoMovimiento Concepto { get; set; }
    public Guid? DenominacionId { get; set; }
    public Guid? PremioId { get; set; }
    public int Cantidad { get; set; } = 1;

    /// <summary>Monto directo capturado, solo para Terminal y Transferencia (no tienen denominación).</summary>
    public decimal? Monto { get; set; }
}

public class RegistrarCierreDiarioRequest
{
    public DateOnly Fecha { get; set; }
    public IList<LineaCierreRequest> Lineas { get; set; } = new List<LineaCierreRequest>();
    public IList<Guid> GastoIds { get; set; } = new List<Guid>();
}

public class CierreDiarioDetalleDto
{
    public ConceptoMovimiento Concepto { get; set; }
    public string? DenominacionNombre { get; set; }
    public string? PremioNombre { get; set; }
    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class CierreDiarioDto
{
    public Guid Id { get; set; }
    public Guid LocalId { get; set; }
    public DateOnly Fecha { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public IList<CierreDiarioDetalleDto> Detalles { get; set; } = new List<CierreDiarioDetalleDto>();
    public decimal TotalReportado { get; set; }
    public decimal TotalEsperado { get; set; }
    public decimal Diferencia { get; set; }
    public EstadoCierre Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}
