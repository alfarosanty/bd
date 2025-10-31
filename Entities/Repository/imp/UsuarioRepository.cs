using BlumeAPI;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext context;

    public UsuarioRepository(AppDbContext context)
    {
        this.context = context;
    }

    public Usuario? ObtenerPorNombre(string nombreUsuario)
    {
        return context.Usuarios.FirstOrDefault(u => u.UserName == nombreUsuario);
    }

    public void Crear(Usuario usuario)
    {
        usuario.Contrasenia = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasenia);
        context.Usuarios.Add(usuario);
        context.SaveChanges();

    }

    public void Actualizar(Usuario usuario)
    {
        context.Usuarios.Update(usuario);
        context.SaveChanges();
    }

    public void Eliminar(int id)
    {
        var usuario = context.Usuarios.Find(id);
        if (usuario != null)
        {
            context.Usuarios.Remove(usuario);
            context.SaveChanges();
        }
    }

    public List<Usuario> ObtenerTodos()
    {
        return context.Usuarios.ToList();
    }

    public Usuario? ObtenerPorId(int idUsuario)
    {
        return context.Usuarios.Find(idUsuario);
    }
}
