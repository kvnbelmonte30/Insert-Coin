using Maquinitas.Domain.Common;

namespace Maquinitas.Api.Dtos.Gastos;

public class GastoDto
{
    public Guid Id { get; set; }
    public Guid LocalId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public TipoGasto Tipo { get; set; }
    public decimal Monto { get; set; }
    public DateOnly Fecha { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public bool TieneEvidencia { get; set; }
    public IList<string> EvidenciaUrls { get; set; } = new List<string>();
    public Guid? CierreDiarioId { get; set; }
}

public class CrearGastoRequest
{
    public Guid LocalId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public TipoGasto Tipo { get; set; } = TipoGasto.General;
    public decimal Monto { get; set; }
    public DateOnly Fecha { get; set; }
}
