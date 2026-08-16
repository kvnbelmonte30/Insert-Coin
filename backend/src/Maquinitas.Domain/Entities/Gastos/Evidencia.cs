using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Gastos;

public class Evidencia : EntityBase
{
    public Guid GastoId { get; set; }
    public Gasto Gasto { get; set; } = null!;

    public string RutaArchivo { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
}
