using BlumeAPI;
using BlumeAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<ArticuloEntity> Articulos { get; set; }
    public DbSet<ArticuloPrecioEntity> ArticuloPrecios { get; set; }

    public DbSet<ColorEntity> Colores { get; set; }
    public DbSet<MedidaEntity> Medidas { get; set; }
    public DbSet<SubFamiliaEntity> SubFamilias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>();

        modelBuilder.ApplyConfiguration(new ArticuloConfiguration());
        modelBuilder.ApplyConfiguration(new ArticuloPrecioConfiguration());
        modelBuilder.ApplyConfiguration(new ColorConfiguration());
        modelBuilder.ApplyConfiguration(new MedidaConfiguration());
        modelBuilder.ApplyConfiguration(new SubFamiliaConfiguration());
    }
}
