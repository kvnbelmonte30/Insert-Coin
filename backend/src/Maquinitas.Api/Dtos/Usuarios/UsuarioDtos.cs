namespace Maquinitas.Api.Dtos.Usuarios;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public IList<Guid> LocalIds { get; set; } = new List<Guid>();
}

public class CrearUsuarioRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public IList<Guid> LocalIds { get; set; } = new List<Guid>();
}

public class ActualizarEstadoUsuarioRequest
{
    public bool Activo { get; set; }
}

public class AsignarLocalesRequest
{
    public IList<Guid> LocalIds { get; set; } = new List<Guid>();
}
