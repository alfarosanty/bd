using BlumeAPI;

public interface IUsuarioRepository
{
    Usuario? ObtenerPorNombre(string nombreUsuario);
    Usuario? ObtenerPorId(int idUsuario);

    void Crear(Usuario usuario);
    void Actualizar(Usuario usuario);
    void Eliminar(int id);
    List<Usuario> ObtenerTodos();
}
