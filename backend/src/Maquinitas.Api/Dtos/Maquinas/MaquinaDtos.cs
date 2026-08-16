using Maquinitas.Domain.Common;

namespace Maquinitas.Api.Dtos.Maquinas;

public class PropietarioDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
}

public class GuardarPropietarioRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}

public class TipoMaquinaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class CrearTipoMaquinaRequest
{
    public string Nombre { get; set; } = string.Empty;
}

public class MaquinaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Guid TipoMaquinaId { get; set; }
    public string TipoMaquinaNombre { get; set; } = string.Empty;
    public Guid LocalId { get; set; }
    public string LocalNombre { get; set; } = string.Empty;
    public Guid PropietarioId { get; set; }
    public string PropietarioNombre { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public EstadoMaquina Estado { get; set; }
    public DateTime FechaAlta { get; set; }
    public bool Activo { get; set; }
}

public class CrearMaquinaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public Guid TipoMaquinaId { get; set; }
    public Guid LocalId { get; set; }
    public Guid PropietarioId { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
}

public class ActualizarMaquinaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public Guid PropietarioId { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public bool Activo { get; set; }
}

public class CambiarEstadoMaquinaRequest
{
    public EstadoMaquina Estado { get; set; }
    public string? Comentario { get; set; }
}

public class HistorialEstadoDto
{
    public EstadoMaquina EstadoAnterior { get; set; }
    public EstadoMaquina EstadoNuevo { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string? Comentario { get; set; }
    public DateTime Fecha { get; set; }
}
