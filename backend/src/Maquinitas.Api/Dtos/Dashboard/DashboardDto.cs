namespace Maquinitas.Api.Dtos.Dashboard;

public class DashboardDto
{
    public int Locales { get; set; }
    public int Maquinas { get; set; }
    public int Empleados { get; set; }
    public int CierresCorrectos { get; set; }
    public int CierresConDiferencia { get; set; }
    public int MaquinasAveriadas { get; set; }
    public int MaquinasEnReparacion { get; set; }
    public int GastosPendientesEvidencia { get; set; }
    public decimal TotalAcumuladoSemanaActual { get; set; }
    public IList<TotalPorLocalDto> TotalesPorLocal { get; set; } = new List<TotalPorLocalDto>();
}

public class TotalPorLocalDto
{
    public Guid LocalId { get; set; }
    public string LocalNombre { get; set; } = string.Empty;
    public int SemanaNumero { get; set; }
    public decimal TotalAcumulado { get; set; }
}
