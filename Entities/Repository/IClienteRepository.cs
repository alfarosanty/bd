namespace BlumeAPI.Repository;

public interface IClienteRepository
{
    Task<PagedResult<Cliente>> GetClientesAsync(int? page, int? pageSize, FiltrosClienteDTO filtros);
    Task<Cliente?> GetById(int idCliente);
    Task<List<CondicionFiscal>> GetCondicionFiscalsAsync();
    void Add(Cliente cliente);
    void Update(Cliente cliente);
}