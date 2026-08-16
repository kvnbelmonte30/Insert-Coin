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

    public static bool HasAccessToLocal(ClaimsPrincipal user, Guid localId) =>
        IsAdmin(user) || GetLocalIds(user).Contains(localId);
}
