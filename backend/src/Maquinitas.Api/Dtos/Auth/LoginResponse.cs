namespace Maquinitas.Api.Dtos.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEn { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public IList<LocalResumenDto> Locales { get; set; } = new List<LocalResumenDto>();
}

public class LocalResumenDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
