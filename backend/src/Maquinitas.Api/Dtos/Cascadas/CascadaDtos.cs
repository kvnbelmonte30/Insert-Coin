namespace Maquinitas.Api.Dtos.Cascadas;

public class MaquinaPremioDto
{
    public Guid Id { get; set; }
    public Guid PremioId { get; set; }
    public string PremioNombre { get; set; } = string.Empty;
    public decimal PremioDenominacion { get; set; }
    public int CantidadAsignada { get; set; }
}

public class ConfigurarPremioRequest
{
    public Guid PremioId { get; set; }
    public int CantidadAsignada { get; set; }
}

public class ConteoInventarioRequest
{
    public IList<ConteoLineaRequest> Lineas { get; set; } = new List<ConteoLineaRequest>();
}

public class ConteoLineaRequest
{
    public Guid PremioId { get; set; }
    public int CantidadEncontrada { get; set; }
}

public class InventarioPremioDetalleDto
{
    public string PremioNombre { get; set; } = string.Empty;
    public int CantidadConfigurada { get; set; }
    public int CantidadEncontrada { get; set; }
    public int Diferencia { get; set; }
}

public class InventarioPremioDto
{
    public Guid Id { get; set; }
    public Guid MaquinaId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public IList<InventarioPremioDetalleDto> Detalles { get; set; } = new List<InventarioPremioDetalleDto>();
}
