namespace BlumeAPI.Services;
using BlumeAPI;
using BlumeAPI.Models;

public interface IUsuarioService
{
    Usuario? ValidarUsuario(string username, string password);
    Usuario CrearUsuario(Usuario usuario);
    Usuario ObtenerPorId(int idUsuario);
    List<Usuario> ObtenerTodos();
    void EliminarUsuario(int id);
}
