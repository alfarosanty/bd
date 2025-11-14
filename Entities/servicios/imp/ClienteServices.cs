using Npgsql;
using BlumeAPI;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Cliente> GetClienteAsync(int id)
    {
        return await Task.Run(() => _clienteRepository.GetClienteAsync(id));
    }

    public async Task<List<Cliente>> ListarClientesAsync()
    {
        return await Task.Run(() => _clienteRepository.ListarClientesAsync());
    }

    public async Task<Cliente> CrearAsync(Cliente cliente)
    {
        return await Task.Run(() => _clienteRepository.CrearAsync(cliente));
    }

    public async Task<int> ActualizarAsync(Cliente cliente)
    {
        return await Task.Run(() => _clienteRepository.ActualizarAsync(cliente));
    }

    public async Task<List<CondicionFiscal>> GetCondicionesFiscalesAsync()
    {
        return await Task.Run(() => _clienteRepository.GetCondicionesFiscalesAsync());
    }
}
