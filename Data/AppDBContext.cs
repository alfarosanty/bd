using Microsoft.EntityFrameworkCore;
using BlumeAPI.Data.Entities;
using BlumeAPI;
using BlumeAPI.Data.Configurations;
using BlumeAPI.Entities.clases.modelo;




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
    public DbSet<ClienteEntity> Clientes { get; set; }
    public DbSet<FacturaEntity> Facturas { get; set; }
    public DbSet<ArticuloFacturaEntity> ArticuloFacturas { get; set; }
    public DbSet<CondicionFiscalEntity> CondicionesFiscales { get; set; }
    public DbSet<NotaDeCredito> NotasDeCredito { get; set; }
    public DbSet<ArticuloNotaCredito> ArticulosNotaCredito { get; set; }

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
        modelBuilder.ApplyConfiguration(new ClienteConfiguration());
        modelBuilder.ApplyConfiguration(new FacturaConfiguration());
        modelBuilder.ApplyConfiguration(new ArticuloFacturaConfiguration());
        modelBuilder.ApplyConfiguration(new CondicionFiscalConfiguration());
        modelBuilder.ApplyConfiguration(new NotaDeCreditoConfiguration());
        modelBuilder.ApplyConfiguration(new ArticuloNotaCreditoConfiguration());
    }
}