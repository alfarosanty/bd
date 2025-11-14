using Microsoft.EntityFrameworkCore;
using BlumeAPI.Models;
using BlumeAPI;
using BlumeApi.Models; // Ajustá si tus modelos están en otros namespaces

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // === ENTIDADES ===
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Color> Colores { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<CondicionFiscal> CondicionesFiscales { get; set; }
    public DbSet<Articulo> Articulos { get; set; }
    public DbSet<ArticuloPrecio> ArticulosPrecio { get; set; }
    public DbSet<ArticuloFactura> ArticulosFactura { get; set; }
    public DbSet<Factura> Facturas { get; set; }
    public DbSet<ArticuloPresupuesto> ArticulosPresupuesto { get; set; }
    public DbSet<Presupuesto> Presupuestos { get; set; }
    public DbSet<EstadoPresupuesto> EstadosPresupuesto { get; set; }
    public DbSet<Taller> Talleres { get; set; }
    public DbSet<Ingreso> Ingresos { get; set; }
    public DbSet<ArticuloIngreso> ArticulosIngreso { get; set; }
    public DbSet<PedidoProduccion> PedidosProduccion { get; set; }
    public DbSet<PedidoProduccionArticulo> PedidosProduccionArticulos { get; set; }
    public DbSet<PedidoProduccionIngresoDetalle> PedidosProduccionIngresoDetalles { get; set; }
    public DbSet<EstadoPedidoProduccion> EstadosPedidoProduccion { get; set; }
    public DbSet<Medida> Medidas { get; set; }
    public DbSet<SubFamilia> SubFamilias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Guarda el enum Rol como texto
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>();

        // Relaciones personalizadas o restricciones adicionales si las necesitás
        modelBuilder.Entity<Factura>()
            .HasOne(f => f.Cliente)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ArticuloFactura>()
            .HasOne(af => af.Articulo)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ArticuloPresupuesto>()
            .HasOne(ap => ap.Articulo)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PedidoProduccion>()
            .HasOne(p => p.Taller)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
