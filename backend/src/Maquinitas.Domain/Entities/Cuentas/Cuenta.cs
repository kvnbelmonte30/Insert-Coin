using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Domain.Entities.Locales;
using Maquinitas.Domain.Entities.Semanas;

namespace Maquinitas.Domain.Entities.Cuentas;

/// <summary>La cuenta que el administrador establece para un local (sección 6-11). Se crea una nueva por cada semana.</summary>
public class Cuenta : EntityBase
{
    public Guid LocalId { get; set; }
    public Local Local { get; set; } = null!;

    public Guid SemanaId { get; set; }
    public Semana Semana { get; set; } = null!;

    public Guid CreadoPorUsuarioId { get; set; }
    public ApplicationUser CreadoPorUsuario { get; set; } = null!;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public bool Activa { get; set; } = true;

    public ICollection<CuentaDetalle> Detalles { get; set; } = new List<CuentaDetalle>();
    public ICollection<CuentaModificacionLog> Modificaciones { get; set; } = new List<CuentaModificacionLog>();
}
