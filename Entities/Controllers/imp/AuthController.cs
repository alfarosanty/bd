
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlumeAPI.Models;
using BlumeAPI.Services;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly IUsuarioService usuarioService; // campo privado y readonly

    public AuthController(IUsuarioService usuarioService)
    {
        this.usuarioService = usuarioService;
    }



[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] UsuarioLoginRequest userRequest)
{
    // Delegamos toda la validación al servicio
    Usuario? usuario = usuarioService.ValidarUsuario(userRequest.userName, userRequest.contrasenia);

    if (usuario == null)
        return Unauthorized("Usuario o contraseña incorrectos");

var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, usuario.UserName), // este debería ser el "identificador"
    new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
    new Claim("Nombre", usuario.Nombre),
    new Claim("Apellido", usuario.Apellido),
};


    var identity = new ClaimsIdentity(claims, "MiCookieAuth");
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync("MiCookieAuth", principal);

    return Ok(usuario);
}


    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MiCookieAuth");
        return Ok(new { message = "Logout correcto" });
    }

[Authorize]
[HttpGet("me")]
public IActionResult Me()
{
    Console.WriteLine("🔍 /me fue llamado. Usuario autenticado: " + User.Identity?.Name);
    return Ok(new
    {
        userName = User.Identity?.Name,
        roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value),
        nombre = User.Claims.FirstOrDefault(c => c.Type == "Nombre")?.Value,
        apellido = User.Claims.FirstOrDefault(c => c.Type == "Apellido")?.Value
    });

}

}
