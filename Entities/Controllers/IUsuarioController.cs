using BlumeAPI;
using BlumeAPI.Models;
using Microsoft.AspNetCore.Mvc;

public interface IUsuarioController
{
    Task<IActionResult> GetUsuarios();
    Task<IActionResult> GetUsuario(int id);
    Task<IActionResult> CrearUsuario(Usuario usuario);
    Task<IActionResult> ActualizarUsuario(int id, Usuario usuario);
    Task<IActionResult> EliminarUsuario(int id);
}
