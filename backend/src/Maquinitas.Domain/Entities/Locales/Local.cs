using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Cierres;
using Maquinitas.Domain.Entities.Cuentas;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Maquinas;
using Maquinitas.Domain.Entities.Semanas;

namespace Maquinitas.Domain.Entities.Locales;

public class Local : EntityBase
{
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<UsuarioLocal> UsuarioLocales { get; set; } = new List<UsuarioLocal>();
    public ICollection<Maquina> Maquinas { get; set; } = new List<Maquina>();
    public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    public ICollection<Semana> Semanas { get; set; } = new List<Semana>();
}
