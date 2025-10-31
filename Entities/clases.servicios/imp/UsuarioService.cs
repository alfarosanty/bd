

using BlumeAPI;
using BlumeAPI.Models;
using BlumeAPI.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        this.usuarioRepository = usuarioRepository;
    }

    public Usuario? ValidarUsuario(string username, string password)
    {
        var usuario = usuarioRepository.ObtenerPorNombre(username);
        if (usuario == null) return null;

        return BCrypt.Net.BCrypt.Verify(password, usuario.Contrasenia) ? usuario : null;
    }

    public Usuario CrearUsuario(Usuario usuario)
    {
        usuarioRepository.Crear(usuario);
        return usuario;
    }

    public List<Usuario> ObtenerTodos() => usuarioRepository.ObtenerTodos();

    public void EliminarUsuario(int id) => usuarioRepository.Eliminar(id);

    public Usuario ObtenerPorId(int idUsuario)
    {
        Usuario? usuario = usuarioRepository.ObtenerPorId(idUsuario);
        if (usuario == null) throw new Exception("Usuario no encontrado");
        return usuario;
    }
}
