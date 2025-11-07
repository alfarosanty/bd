using BlumeAPI;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Color> Colores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Solo tratamos el enum Rol para guardarlo como texto
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>();
    }
}
