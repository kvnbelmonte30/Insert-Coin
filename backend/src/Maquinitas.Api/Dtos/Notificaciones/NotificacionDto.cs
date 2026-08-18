using Maquinitas.Domain.Common;

namespace Maquinitas.Api.Dtos.Notificaciones;

public class NotificacionDto
{
    public Guid Id { get; set; }
    public Guid? LocalId { get; set; }
    public string? LocalNombre { get; set; }
    public TipoNotificacion Tipo { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public Guid? ReferenciaId { get; set; }
    public bool Leida { get; set; }
    public DateTime FechaCreacion { get; set; }
}
