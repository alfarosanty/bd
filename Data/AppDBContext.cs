using Microsoft.EntityFrameworkCore;
using BlumeAPI;
using BlumeAPI.Data.Configurations;
using BlumeAPI.Entities;




public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Articulo> Articulos { get; set; }
    public DbSet<ArticuloPrecio> ArticuloPrecios { get; set; }
    public DbSet<Color> Colores { get; set; }
    public DbSet<Medida> Medidas { get; set; }
    public DbSet<SubFamilia> SubFamilias { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Factura> Facturas { get; set; }
    public DbSet<ArticuloFactura> ArticuloFacturas { get; set; }
    public DbSet<CondicionFiscal> CondicionesFiscales { get; set; }
    public DbSet<NotaDeCredito> NotasDeCredito { get; set; }
    public DbSet<ArticuloNotaCredito> ArticulosNotaCredito { get; set; }
    public DbSet<Presupuesto> Presupuestos { get; set; }
    public DbSet<ArticuloPresupuesto> ArticulosPresupuesto { get; set; }
    public DbSet<EstadoPresupuesto> EstadosPresupuesto { get; set; }
    public DbSet<PedidoProduccion> PedidosProduccion { get; set; }
    public DbSet<PedidoProduccionArticulo> PedidoProduccionArticulos { get; set; }
    public DbSet<EstadoPedidoProduccion> EstadosPedidoProduccion { get; set; }
    public DbSet<Taller> Talleres { get; set; }
    public DbSet<Ingreso> Ingresos { get; set; }
    public DbSet<ArticuloIngreso> ArticulosIngreso { get; set; }
    public DbSet<PedidoProduccionIngresoDetalle> PedidoProduccionIngresoDetalles { get; set; }

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
        modelBuilder.ApplyConfiguration(new PresupuestoConfiguration());
        modelBuilder.ApplyConfiguration(new ArticuloPresupuestoConfiguration());
        modelBuilder.ApplyConfiguration(new EstadoPresupuestoConfiguration());
        modelBuilder.ApplyConfiguration(new PedidoProduccionConfiguration());
        modelBuilder.ApplyConfiguration(new PedidoProduccionArticuloConfiguration());
        modelBuilder.ApplyConfiguration(new EstadoPedidoProduccionConfiguration());
        modelBuilder.ApplyConfiguration(new TallerConfiguration());
        modelBuilder.ApplyConfiguration(new IngresoConfiguration());
        modelBuilder.ApplyConfiguration(new ArticuloIngresoConfiguration());
        modelBuilder.ApplyConfiguration(new PedidoProduccionIngresoDetalleConfiguration());

    }
}