using BlumeAPI;
using BlumeAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetClienteAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.CondicionFiscal)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Cliente>> ListarClientesAsync()
    {
        return await _context.Clientes
            .Include(c => c.CondicionFiscal)
            .ToListAsync();
    }

    public async Task<Cliente> CrearAsync(Cliente cliente)
    {
        if (await _context.Clientes.AnyAsync(c => c.Cuit == cliente.Cuit))
            throw new InvalidOperationException("Ya existe un cliente con ese CUIT.");

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task<int> ActualizarAsync(Cliente cliente)
    {
        var existing = await _context.Clientes.FindAsync(cliente.Id);
        if (existing == null)
            throw new KeyNotFoundException($"No se encontró el cliente con ID {cliente.Id}");

        _context.Entry(existing).CurrentValues.SetValues(cliente);
        await _context.SaveChangesAsync();
        return 1;
    }

    public async Task<List<CondicionFiscal>> GetCondicionesFiscalesAsync()
    {
        return await _context.CondicionesFiscales
            .OrderBy(cf => cf.Descripcion)
            .ToListAsync();
    }
}
