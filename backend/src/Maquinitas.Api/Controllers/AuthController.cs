using Maquinitas.Api.Dtos.Auth;
using Maquinitas.Api.Services;
using Maquinitas.Domain.Entities.Identity;
using Maquinitas.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinitas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _tokenService;
    private readonly ApplicationDbContext _db;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService tokenService,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _db = db;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user is null || !user.Activo)
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var localIds = await _db.UsuarioLocales
            .Where(ul => ul.UsuarioId == user.Id)
            .Select(ul => ul.LocalId)
            .ToListAsync();

        var locales = await _db.Locales
            .Where(l => localIds.Contains(l.Id))
            .Select(l => new LocalResumenDto { Id = l.Id, Nombre = l.Nombre })
            .ToListAsync();

        var token = _tokenService.GenerateToken(user, roles, localIds);

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiraEn = DateTime.UtcNow.AddMinutes(480),
            UsuarioId = user.Id,
            Nombre = user.Nombre,
            UserName = user.UserName ?? string.Empty,
            Roles = roles,
            Locales = locales,
            DebeCambiarContrasena = user.DebeCambiarContrasena
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<LoginResponse>> Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var localIds = await _db.UsuarioLocales.Where(ul => ul.UsuarioId == user.Id).Select(ul => ul.LocalId).ToListAsync();
        var locales = await _db.Locales.Where(l => localIds.Contains(l.Id))
            .Select(l => new LocalResumenDto { Id = l.Id, Nombre = l.Nombre }).ToListAsync();

        return Ok(new LoginResponse
        {
            UsuarioId = user.Id,
            Nombre = user.Nombre,
            UserName = user.UserName ?? string.Empty,
            Roles = roles,
            Locales = locales,
            DebeCambiarContrasena = user.DebeCambiarContrasena
        });
    }

    [HttpPost("cambiar-contrasena")]
    [Authorize]
    public async Task<IActionResult> CambiarContrasena(CambiarContrasenaRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Unauthorized();

        var result = await _userManager.ChangePasswordAsync(user, request.ContrasenaActual, request.ContrasenaNueva);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
        }

        user.DebeCambiarContrasena = false;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}
