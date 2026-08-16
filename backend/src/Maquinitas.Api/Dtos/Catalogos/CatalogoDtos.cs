using Maquinitas.Domain.Common;

namespace Maquinitas.Api.Dtos.Catalogos;

public class DenominacionDto
{
    public Guid Id { get; set; }
    public TipoDenominacion Tipo { get; set; }
    public decimal Valor { get; set; }
    public decimal? ValorPorBolsa { get; set; }
    public bool Activo { get; set; }
}

public class GuardarDenominacionRequest
{
    public TipoDenominacion Tipo { get; set; }
    public decimal Valor { get; set; }
    public decimal? ValorPorBolsa { get; set; }
}

public class PremioDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Denominacion { get; set; }
    public bool Activo { get; set; }
}

public class GuardarPremioRequest
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Denominacion { get; set; }
}
