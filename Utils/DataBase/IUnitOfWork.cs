using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;

public interface IUnitOfWork : IDisposable
{
    public AppDbContext Context {  get; }

    // Propiedades directas
    public IIngresoRepository Ingresos { get; }
    public IArticuloRepository Articulos { get; }
    public IPedidoProduccionRepository Pedidos { get; }
    public IPresupuestoRepository Presupuestos { get; }
    public IFacturaRepository Facturas { get; }
    public ISubfamiliaRepository Subfamilias { get; }
    public IColorRepository Colores { get; }
    public IMedidaRepository Medidas { get; }
    public IClienteRepository Clientes { get; }
    public IARCARepository Arca { get; }

    // Métodos de persistencia y transaccionalidad
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}