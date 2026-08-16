namespace Maquinitas.Api.Dtos.Locales;

public class LocalDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int SemanaActualNumero { get; set; }
    public Guid? SemanaActualId { get; set; }
}

public class CrearLocalRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}

public class ActualizarLocalRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
