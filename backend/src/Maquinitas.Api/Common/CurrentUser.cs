using System.Security.Claims;

namespace Maquinitas.Api.Common;

public static class CurrentUser
{
    public static Guid GetId(ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return value is null ? Guid.Empty : Guid.Parse(value);
    }

    public static string GetNombre(ClaimsPrincipal user) => user.FindFirst("nombre")?.Value ?? string.Empty;

    public static bool IsAdmin(ClaimsPrincipal user) => user.IsInRole(Maquinitas.Domain.Common.Roles.Administrador);

    public static IReadOnlyCollection<Guid> GetLocalIds(ClaimsPrincipal user) =>
        user.FindAll("local_id").Select(c => Guid.Parse(c.Value)).ToList();

    /// <summary>
    /// Un administrador sin locales asignados ve todo el sistema (super admin).
    /// Un administrador con uno o más locales asignados queda restringido a esos locales, igual que un empleado.
    /// </summary>
    public static bool HasAccessToLocal(ClaimsPrincipal user, Guid localId)
    {
        var localIds = GetLocalIds(user);
        if (IsAdmin(user) && localIds.Count == 0) return true;
        return localIds.Contains(localId);
    }

    /// <summary>
    /// Locales visibles para listados. Null significa "todos" (super admin sin locales asignados);
    /// una lista (posiblemente vacía) significa que el resultado debe filtrarse a esos locales.
    /// </summary>
    public static IReadOnlyCollection<Guid>? GetVisibleLocalIds(ClaimsPrincipal user)
    {
        var localIds = GetLocalIds(user);
        return IsAdmin(user) && localIds.Count == 0 ? null : localIds;
    }
}
