using System.Text.Json;
using Maquinitas.Domain.Entities.Auditoria;
using Maquinitas.Infrastructure.Data;

namespace Maquinitas.Api.Services;

public class AuditoriaService
{
    private readonly ApplicationDbContext _db;

    public AuditoriaService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task RegistrarAsync(
        Guid usuarioId,
        string usuarioNombre,
        Guid? localId,
        string accion,
        string entidad,
        string entidadId,
        object? valorAnterior = null,
        object? valorNuevo = null)
    {
        var evento = new AuditoriaEvento
        {
            UsuarioId = usuarioId,
            UsuarioNombre = usuarioNombre,
            LocalId = localId,
            Accion = accion,
            Entidad = entidad,
            EntidadId = entidadId,
            ValorAnterior = valorAnterior is null ? null : JsonSerializer.Serialize(valorAnterior),
            ValorNuevo = valorNuevo is null ? null : JsonSerializer.Serialize(valorNuevo)
        };

        _db.AuditoriaEventos.Add(evento);
        await _db.SaveChangesAsync();
    }
}
