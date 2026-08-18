using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;

namespace Maquinitas.Domain.Entities.Maquinas;

/// <summary>
/// Corte financiero de una máquina específica: el total sacado de ELLA. Es un registro adicional,
/// independiente del cierre diario/semanal por local.
/// </summary>
public class CorteMaquina : EntityBase
{
    public Guid MaquinaId { get; set; }
    public Maquina Maquina { get; set; } = null!;

    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public Guid RegistradoPorId { get; set; }
    public ApplicationUser RegistradoPor { get; set; } = null!;

    public DateOnly Fecha { get; set; }
    public string? Comentario { get; set; }
    public decimal Total { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
