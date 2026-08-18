using Microsoft.AspNetCore.Identity;

namespace Maquinitas.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public bool DebeCambiarContrasena { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<UsuarioLocal> UsuarioLocales { get; set; } = new List<UsuarioLocal>();
}
