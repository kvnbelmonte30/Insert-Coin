using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Locales;

namespace Maquinitas.Domain.Entities.Semanas;

public class Semana : EntityBase
{
    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public int Numero { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public EstadoSemana Estado { get; set; } = EstadoSemana.Abierta;
}
