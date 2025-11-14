using Npgsql;
using BlumeAPI;

public interface IClienteService
{
    Task<Cliente> GetClienteAsync(int id);
    Task<List<Cliente>> ListarClientesAsync();
    Task<Cliente> CrearAsync(Cliente cliente);
    Task<int> ActualizarAsync(Cliente cliente);
    Task<List<CondicionFiscal>> GetCondicionesFiscalesAsync();
}
