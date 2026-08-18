namespace Maquinitas.Api.Dtos.Maquinas;

public class RegistrarCorteMaquinaRequest
{
    public DateOnly Fecha { get; set; }
    public string? Comentario { get; set; }
    public decimal Total { get; set; }
}

public class CorteMaquinaDto
{
    public Guid Id { get; set; }
    public Guid MaquinaId { get; set; }
    public string MaquinaNombre { get; set; } = string.Empty;
    public Guid LocalId { get; set; }
    public string LocalNombre { get; set; } = string.Empty;
    public string RegistradoPorNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string? Comentario { get; set; }
    public decimal Total { get; set; }
    public DateTime FechaCreacion { get; set; }
}
