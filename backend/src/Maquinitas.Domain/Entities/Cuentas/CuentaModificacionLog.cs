using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;

namespace Maquinitas.Domain.Entities.Cuentas;

/// <summary>Registro de auditoría específico de cambios a la cuenta, usado para la notificación al empleado (sección 10-11).</summary>
public class CuentaModificacionLog : EntityBase
{
    public Guid CuentaId { get; set; }
    public Cuenta Cuenta { get; set; } = null!;

    public Guid UsuarioId { get; set; }
    public ApplicationUser Usuario { get; set; } = null!;

    public string Concepto { get; set; } = string.Empty;
    public string ValorAnterior { get; set; } = string.Empty;
    public string ValorNuevo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public bool NotificacionEnviada { get; set; } = false;
}
