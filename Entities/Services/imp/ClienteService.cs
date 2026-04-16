using BlumeAPI.Services;

public class ClienteService : IClienteService
{
    
    private readonly IUnitOfWork _unitOfWork;

    public ClienteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<Cliente>> GetClientesAsync(int? page, int? pageSize, FiltrosClienteDTO filtros)
    {
        return await _unitOfWork.Clientes.GetClientesAsync(page, pageSize, filtros);
        
    }

    public async Task<Cliente> GetById(int id)
    {
        
        var clienteEncontrado = await _unitOfWork.Clientes.GetById(id);
        if (clienteEncontrado != null)
        {
            return clienteEncontrado;
        }
        else
        {
            throw new Exception("Cliente no encontrado en la BD");
        }
        
    }

    public async Task<List<CondicionFiscal>> GetCondicionFiscalsAsync()
    {
        return await _unitOfWork.Clientes.GetCondicionFiscalsAsync();
    }

    public async Task<int> Guardar(Cliente cliente)
    {
        if(cliente.CondicionFiscal != null)
        {
            cliente.IdCondicionFiscal = cliente.CondicionFiscal.IdCondicion;
            cliente.CondicionFiscal = null;
        }

        if (cliente.Id == 0)
        {
            _unitOfWork.Clientes.Add(cliente);
        }
        else
        {
            _unitOfWork.Clientes.Update(cliente);
        }
                
        await _unitOfWork.SaveChangesAsync(); 

        return cliente.Id;    
    }


}