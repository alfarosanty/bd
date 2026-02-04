using BlumeAPI;
using BlumeAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<ArticuloEntity> Articulos { get; set; }
    public DbSet<ArticuloPrecioEntity> ArticuloPrecios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Solo tratamos el enum Rol para guardarlo como texto
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>();

        modelBuilder.ApplyConfiguration(new ArticuloConfiguration());
        modelBuilder.ApplyConfiguration(new ArticuloPrecioConfiguration());

    }

}
