using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    // Propiedades directas
    public IIngresoRepository Ingresos { get; }
    public IArticuloRepository Articulos { get; }
    public IPedidoProduccionRepository Pedidos { get; }
    public IPresupuestoRepository Presupuestos { get; }
    public IFacturaRepository Facturas { get; }
    public ISubfamiliaRepository Subfamilias { get; }
    public IColorRepository Colores { get; }
    public IMedidaRepository Medidas { get; }
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
        IFacturaRepository facturaRepository
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