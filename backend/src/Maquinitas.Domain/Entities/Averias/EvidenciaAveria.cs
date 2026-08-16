using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Averias;

public class EvidenciaAveria : EntityBase
{
    public Guid ReporteAveriaId { get; set; }
    public ReporteAveria ReporteAveria { get; set; } = null!;

    public string RutaArchivo { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
}
