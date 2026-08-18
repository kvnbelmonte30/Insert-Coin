using Maquinitas.Api.Common;
using Maquinitas.Api.Dtos.Usuarios;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Common;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuditoriaService _auditoria;

    public UsuariosController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, AuditoriaService auditoria)
    {
        _db = db;
        _userManager = userManager;
        _auditoria = auditoria;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
    {
        var usuarios = await _db.Users.Include(u => u.UsuarioLocales).Include(u => u.Propietario).OrderBy(u => u.Nombre).ToListAsync();

        var result = new List<UsuarioDto>();
        foreach (var u in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(u);
            result.Add(ToDto(u, roles));
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Create(CrearUsuarioRequest request)
    {
        if (request.Rol != Roles.Administrador && request.Rol != Roles.Empleado)
        {
            return BadRequest(new { message = "Rol inválido. Debe ser Administrador o Empleado." });
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            Nombre = request.Nombre,
            Activo = true,
            EmailConfirmed = true,
            PropietarioId = request.PropietarioId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
        }

        await _userManager.AddToRoleAsync(user, request.Rol);

        foreach (var localId in request.LocalIds.Distinct())
        {
            _db.UsuarioLocales.Add(new UsuarioLocal { UsuarioId = user.Id, LocalId = localId });
        }
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(
            CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Usuario creado", nameof(ApplicationUser), user.Id.ToString(), null,
            new { user.UserName, user.Nombre, request.Rol, request.LocalIds, request.PropietarioId });

        var creado = await _db.Users.Include(u => u.Propietario).FirstAsync(u => u.Id == user.Id);
        return CreatedAtAction(nameof(GetAll), ToDto(creado, new List<string> { request.Rol }));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> Update(Guid id, EditarUsuarioRequest request)
    {
        if (request.Rol != Roles.Administrador && request.Rol != Roles.Empleado)
        {
            return BadRequest(new { message = "Rol inválido. Debe ser Administrador o Empleado." });
        }

        var user = await _db.Users.Include(u => u.UsuarioLocales).FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        var rolesAnteriores = await _userManager.GetRolesAsync(user);
        var anterior = new { user.Nombre, user.Email, Roles = rolesAnteriores, LocalIds = user.UsuarioLocales.Select(ul => ul.LocalId) };

        user.Nombre = request.Nombre;
        user.PropietarioId = request.PropietarioId;
        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            await _userManager.SetEmailAsync(user, request.Email);
        }
        await _userManager.UpdateAsync(user);

        if (!rolesAnteriores.Contains(request.Rol))
        {
            await _userManager.RemoveFromRolesAsync(user, rolesAnteriores);
            await _userManager.AddToRoleAsync(user, request.Rol);
        }

        _db.UsuarioLocales.RemoveRange(user.UsuarioLocales);
        foreach (var localId in request.LocalIds.Distinct())
        {
            _db.UsuarioLocales.Add(new UsuarioLocal { UsuarioId = id, LocalId = localId });
        }
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(
            CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Usuario modificado", nameof(ApplicationUser), user.Id.ToString(), anterior,
            new { request.Nombre, request.Email, request.Rol, request.LocalIds });

        var completo = await _db.Users.Include(u => u.UsuarioLocales).Include(u => u.Propietario).FirstAsync(u => u.Id == id);
        var roles = await _userManager.GetRolesAsync(completo);
        return Ok(ToDto(completo, roles));
    }

    [HttpPost("{id:guid}/restablecer-contrasena")]
    public async Task<IActionResult> RestablecerContrasena(Guid id, RestablecerContrasenaRequest request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        var removeResult = await _userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", removeResult.Errors.Select(e => e.Description)) });
        }

        var addResult = await _userManager.AddPasswordAsync(user, request.NuevaContrasena);
        if (!addResult.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", addResult.Errors.Select(e => e.Description)) });
        }

        user.DebeCambiarContrasena = true;
        await _userManager.UpdateAsync(user);

        await _auditoria.RegistrarAsync(
            CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Contraseña restablecida", nameof(ApplicationUser), user.Id.ToString(), null, null);

        return NoContent();
    }

    [HttpPut("{id:guid}/estado")]
    public async Task<IActionResult> UpdateEstado(Guid id, ActualizarEstadoUsuarioRequest request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        var anterior = user.Activo;
        user.Activo = request.Activo;
        await _userManager.UpdateAsync(user);

        await _auditoria.RegistrarAsync(
            CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            request.Activo ? "Usuario activado" : "Usuario desactivado",
            nameof(ApplicationUser), user.Id.ToString(), new { Activo = anterior }, new { request.Activo });

        return NoContent();
    }

    [HttpPut("{id:guid}/locales")]
    public async Task<IActionResult> AsignarLocales(Guid id, AsignarLocalesRequest request)
    {
        var user = await _db.Users.Include(u => u.UsuarioLocales).FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        var anteriores = user.UsuarioLocales.Select(ul => ul.LocalId).ToList();

        _db.UsuarioLocales.RemoveRange(user.UsuarioLocales);
        foreach (var localId in request.LocalIds.Distinct())
        {
            _db.UsuarioLocales.Add(new UsuarioLocal { UsuarioId = id, LocalId = localId });
        }
        await _db.SaveChangesAsync();

        await _auditoria.RegistrarAsync(
            CurrentUser.GetId(User), CurrentUser.GetNombre(User), null,
            "Locales reasignados", nameof(ApplicationUser), user.Id.ToString(),
            new { LocalIds = anteriores }, new { request.LocalIds });

        return NoContent();
    }

    private static UsuarioDto ToDto(ApplicationUser user, IList<string> roles) => new()
    {
        Id = user.Id,
        UserName = user.UserName ?? string.Empty,
        Email = user.Email ?? string.Empty,
        Nombre = user.Nombre,
        Activo = user.Activo,
        Roles = roles,
        LocalIds = user.UsuarioLocales.Select(ul => ul.LocalId).ToList(),
        PropietarioId = user.PropietarioId,
        PropietarioNombre = user.Propietario?.Nombre
    };
}
