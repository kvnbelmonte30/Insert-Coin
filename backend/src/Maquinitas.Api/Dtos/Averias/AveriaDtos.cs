using Maquinitas.Domain.Common;

namespace Maquinitas.Api.Dtos.Averias;

public class ReporteAveriaDto
{
    public Guid Id { get; set; }
    public Guid LocalId { get; set; }
    public Guid MaquinaId { get; set; }
    public string MaquinaNombre { get; set; } = string.Empty;
    public string EmpleadoNombre { get; set; } = string.Empty;
    public string Problema { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public EstadoMaquina EstadoAlReportar { get; set; }
    public DateTime Fecha { get; set; }
    public IList<string> EvidenciaUrls { get; set; } = new List<string>();
}

public class CrearReporteAveriaRequest
{
    public Guid LocalId { get; set; }
    public Guid MaquinaId { get; set; }
    public string Problema { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
