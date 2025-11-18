using Npgsql;
using BlumeAPI;
using BlumeAPI.Models;

public interface IClienteService
{
    Task<Cliente> GetClienteAsync(int id);
    Task<List<Cliente>> ListarClientesAsync();
    Task<int> ActualizarAsync(Cliente cliente);
    Task<List<CondicionFiscal>> GetCondicionesFiscalesAsync();
    Task<Cliente> CrearAsync(Cliente cliente);
}
