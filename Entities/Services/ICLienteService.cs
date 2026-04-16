namespace BlumeAPI.Services;

public interface IClienteService
{
    Task<PagedResult<Cliente>> GetClientesAsync(int? page, int? pageSize, FiltrosClienteDTO filtros);
    Task<Cliente> GetById(int idCliente);
    Task<List<CondicionFiscal>> GetCondicionFiscalsAsync();
    Task<int> Guardar(Cliente cliente); 
    
}