using BlumeAPI.Repository;
using BlumeAPI.Services;

public class PresupuestoService : IPresupuestoService
{
    private readonly IPresupuestoRepository _presupuestoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PresupuestoService(
        IPresupuestoRepository presupuestoRepository,
        IUnitOfWork unitOfWork
        )
    {
        _presupuestoRepository = presupuestoRepository;
        _unitOfWork = unitOfWork;
    }

// Service
    public async Task<PagedResult<Presupuesto>> GetPresupuestosAsync(
        DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize)
    {
        // Ya no retorna List<>, retorna PagedResult<>
        return await _presupuestoRepository.GetAll(desde, hasta, idEstado, page, pageSize);
    }

    public async Task<PagedResult<Presupuesto>> GetPresupuestosByClienteAsync(
        int idCliente, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize)
    {
        return await _presupuestoRepository.GetByCliente(idCliente, desde, hasta, idEstado, page, pageSize);
    }

    public async Task<Presupuesto?> GetPresupuestoByIdAsync(int id)
    {
        return await _presupuestoRepository.GetById(id);
    }

    public async Task<int> CrearPresupuesto(Presupuesto presupuesto)
    {
        if (presupuesto == null) throw new ArgumentNullException(nameof(presupuesto));
        if (presupuesto.Articulos == null || !presupuesto.Articulos.Any())
            throw new BusinessException("El presupuesto debe contener al menos un artículo.");

        try 
        {
            await _unitOfWork.BeginTransactionAsync();

            presupuesto.FechaCreacion = DateTime.Now;

            _unitOfWork.Presupuestos.Crear(presupuesto);

            await _unitOfWork.SaveChangesAsync();
            int nuevoPresupuestoId = presupuesto.Id;
            await _unitOfWork.CommitAsync();

            return nuevoPresupuestoId;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<int> ActualizarPresupuesto(Presupuesto presupuesto)
    {
        if (presupuesto == null) throw new ArgumentNullException(nameof(presupuesto));
        if (presupuesto.Id <= 0) throw new BusinessException("El ID del presupuesto es inválido.");
        if (presupuesto.Articulos == null || !presupuesto.Articulos.Any())
            throw new BusinessException("El presupuesto debe contener al menos un artículo.");

        try 
        {
            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.Presupuestos.Actualizar(presupuesto);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return presupuesto.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<List<EstadoPresupuesto>> GetEstadosPresupuesto()
    {
        return await _unitOfWork.Presupuestos.GetEstados();
    }

}