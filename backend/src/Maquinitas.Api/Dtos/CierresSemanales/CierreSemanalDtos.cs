using Maquinitas.Domain.Common;

namespace Maquinitas.Api.Dtos.CierresSemanales;

public class LineaCierreSemanalRequest
{
    public ConceptoMovimiento Concepto { get; set; }
    public Guid? DenominacionId { get; set; }
    public Guid? PremioId { get; set; }
    public int Cantidad { get; set; } = 1;
    public decimal? Monto { get; set; }
    public bool EsPremioPuesto { get; set; } = false;
}

public class RegistrarCierreSemanalRequest
{
    public IList<LineaCierreSemanalRequest> Lineas { get; set; } = new List<LineaCierreSemanalRequest>();
}

public class CierreSemanalDetalleDto
{
    public ConceptoMovimiento Concepto { get; set; }
    public string? DenominacionNombre { get; set; }
    public string? PremioNombre { get; set; }
    public bool EsPremioPuesto { get; set; }
    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class CierreSemanalDto
{
    public Guid Id { get; set; }
    public Guid SemanaId { get; set; }
    public int SemanaNumero { get; set; }
    public Guid LocalId { get; set; }
    public IList<CierreSemanalDetalleDto> Detalles { get; set; } = new List<CierreSemanalDetalleDto>();
    public decimal TotalReportado { get; set; }
    public decimal TotalEsperado { get; set; }
    public decimal Diferencia { get; set; }
    public EstadoCierre EstadoDiferencia { get; set; }
    public bool Confirmado { get; set; }
    public string CreadoPorNombre { get; set; } = string.Empty;
    public string? ConfirmadoPorNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaConfirmacion { get; set; }
}
