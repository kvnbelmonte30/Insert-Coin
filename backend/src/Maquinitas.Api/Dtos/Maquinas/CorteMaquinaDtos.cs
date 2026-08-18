namespace Maquinitas.Api.Dtos.Maquinas;

public class LineaCorteMaquinaRequest
{
    public Guid? DenominacionId { get; set; }
    public Guid? PremioId { get; set; }
    public int Cantidad { get; set; } = 1;
}

public class RegistrarCorteMaquinaRequest
{
    public DateOnly Fecha { get; set; }
    public string? Comentario { get; set; }
    public IList<LineaCorteMaquinaRequest> Lineas { get; set; } = new List<LineaCorteMaquinaRequest>();
}

public class CorteMaquinaDetalleDto
{
    public string? DenominacionNombre { get; set; }
    public string? PremioNombre { get; set; }
    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class CorteMaquinaDto
{
    public Guid Id { get; set; }
    public Guid MaquinaId { get; set; }
    public string MaquinaNombre { get; set; } = string.Empty;
    public Guid LocalId { get; set; }
    public string LocalNombre { get; set; } = string.Empty;
    public string EmpleadoNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string? Comentario { get; set; }
    public decimal Total { get; set; }
    public DateTime FechaCreacion { get; set; }
    public IList<CorteMaquinaDetalleDto> Detalles { get; set; } = new List<CorteMaquinaDetalleDto>();
}
