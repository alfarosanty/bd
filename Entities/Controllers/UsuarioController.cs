using Microsoft.AspNetCore.Mvc;
using BlumeAPI.Services;
using BlumeAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BlumeAPI;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase // puede implementar IUsuarioController si querés
{
    private readonly IUsuarioService usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        this.usuarioService = usuarioService;
    }

    [HttpGet]
    //[Authorize(Roles = "ADMIN,CONTROL")] // ej. solo estos roles pueden ver todos
    public  IActionResult GetUsuarios()
    {
        var usuarios = usuarioService.ObtenerTodos();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    //[Authorize(Roles = "ADMIN,CONTROL,ARMADO")]
    public IActionResult GetUsuario(int id)
    {
        var usuario =  usuarioService.ObtenerPorId(id);
        if (usuario == null) return NotFound();
        return Ok(usuario);
    }
   

    [HttpPost]
    //[Authorize(Roles = "ADMIN")]
    public IActionResult CrearUsuario([FromBody] Usuario usuario)
    {
        var creado = usuarioService.CrearUsuario(usuario);
        return CreatedAtAction(nameof(GetUsuario), new { id = creado.Id }, creado);
    }

    [HttpDelete("{id}")]
    //[Authorize(Roles = "ADMIN")]
    public IActionResult EliminarUsuario(int id)
    {
        usuarioService.EliminarUsuario(id); // void
        return NoContent();
    }

}
