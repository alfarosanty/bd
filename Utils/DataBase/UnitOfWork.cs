using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public AppDbContext Context => _context;
    public IDbContextTransaction? _transaction;

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

    public UnitOfWork(
        AppDbContext context,
        IIngresoRepository ingresos,
        IArticuloRepository articulos,
        IPedidoProduccionRepository pedidos,
        IPresupuestoRepository presupuestos,
        ISubfamiliaRepository subfamilias,
        IColorRepository colores,
        IMedidaRepository medidas,
        IARCARepository arca,
        IFacturaRepository facturaRepository,
        IClienteRepository clienteRepository
        )
    {
        _context = context;
        Arca = arca;
        Presupuestos = presupuestos;
        Ingresos = ingresos;
        Articulos = articulos;
        Pedidos = pedidos;
        Subfamilias = subfamilias;
        Colores = colores;
        Medidas = medidas;
        Facturas = facturaRepository;
        Clientes = clienteRepository;
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _transaction.CommitAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
        }
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
    }

    public void Dispose()
    {
        
    }
}