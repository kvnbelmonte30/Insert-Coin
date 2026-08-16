using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;

namespace Maquinitas.Domain.Entities.Maquinas;

public class HistorialEstadoMaquina : EntityBase
{
    public Guid MaquinaId { get; set; }
    public Maquina Maquina { get; set; } = null!;

    public EstadoMaquina EstadoAnterior { get; set; }
    public EstadoMaquina EstadoNuevo { get; set; }

    public Guid UsuarioId { get; set; }
    public ApplicationUser Usuario { get; set; } = null!;

    public string? Comentario { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
