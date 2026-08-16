using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Cuentas;

/// <summary>
/// Catálogo configurable de denominaciones (bolsas, monedas, billetes). Nunca se codifican
/// valores fijos en el código: todo se define aquí (sección 6-7 del plan).
/// </summary>
public class Denominacion : EntityBase
{
    public TipoDenominacion Tipo { get; set; }

    /// <summary>Valor nominal: para Moneda/Billete es su valor facial; para Bolsa es la denominación de las monedas que contiene (ej. bolsa "de $10").</summary>
    public decimal Valor { get; set; }

    /// <summary>Solo aplica a Tipo=Bolsa: valor total que representa una bolsa completa (ej. bolsa de $10 con valor unitario $2,000).</summary>
    public decimal? ValorPorBolsa { get; set; }

    public bool Activo { get; set; } = true;
}
