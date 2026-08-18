namespace Maquinitas.Api.Dtos.Dashboard;

public class PropietarioResumenDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int TotalMaquinas { get; set; }
}

public class ConteoDto
{
    public string Etiqueta { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class MaquinaResumenDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoNombre { get; set; } = string.Empty;
    public string LocalNombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int AveriasUltimos30Dias { get; set; }
}

public class AveriaTendenciaDto
{
    public string Periodo { get; set; } = string.Empty;
    public DateOnly InicioSemana { get; set; }
    public int Cantidad { get; set; }
}

public class PropietarioDashboardDto
{
    public Guid PropietarioId { get; set; }
    public string PropietarioNombre { get; set; } = string.Empty;
    public int TotalMaquinas { get; set; }
    public IList<ConteoDto> MaquinasPorEstado { get; set; } = new List<ConteoDto>();
    public IList<ConteoDto> MaquinasPorTipo { get; set; } = new List<ConteoDto>();
    public IList<MaquinaResumenDto> Maquinas { get; set; } = new List<MaquinaResumenDto>();
    public int AveriasUltimos30Dias { get; set; }
    public int AveriasUltimos90Dias { get; set; }
    public IList<AveriaTendenciaDto> AveriasPorSemana { get; set; } = new List<AveriaTendenciaDto>();
    public double? TiempoPromedioReparacionHoras { get; set; }
    public IList<TotalPorLocalDto> TotalesPorLocalOperando { get; set; } = new List<TotalPorLocalDto>();
    public decimal TotalAcumuladoLocalesOperando { get; set; }
}
