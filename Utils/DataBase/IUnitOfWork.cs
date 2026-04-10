using BlumeAPI.Entities.Repository;
using BlumeAPI.Repository;

public interface IUnitOfWork : IDisposable
{
    // Repositorios (esto permite centralizar el acceso)
    IIngresoRepository Ingresos { get; }
    IArticuloRepository Articulos { get; }
    IPedidoProduccionRepository Pedidos { get; }

    ISubfamiliaRepository Subfamilias { get; }
    IColorRepository Colores { get; }
    IMedidaRepository Medidas { get; }

    // Métodos de persistencia y transaccionalidad
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}