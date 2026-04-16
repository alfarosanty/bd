using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Cliente>> GetClientesAsync(int? page, int? pageSize, FiltrosClienteDTO f)
    {
        var query = _context.Clientes
                .Include(c=>c.CondicionFiscal)
                .AsQueryable();

        // 1. Aplicamos filtros dinámicos (igual que antes)
    if (!string.IsNullOrWhiteSpace(f.RazonSocial))
        query = query.Where(c => c.RazonSocial.Contains(f.RazonSocial));

    if (!string.IsNullOrWhiteSpace(f.Telefono))
        query = query.Where(c => c.Telefono!.Contains(f.Telefono));

    if (!string.IsNullOrWhiteSpace(f.Localidad))
        query = query.Where(c => c.Localidad == f.Localidad);

    if (!string.IsNullOrWhiteSpace(f.Provincia))
        query = query.Where(c => c.Provincia == f.Provincia);
    
    if (!string.IsNullOrWhiteSpace(f.Cuit))
        query = query.Where(c => c.Cuit == f.Cuit);
        // ... resto de tus filtros

        // 2. Lógica de Paginación Condicional
        bool esPaginado = page.HasValue && pageSize.HasValue;

        var totalItems = await query.CountAsync();
        
        List<Cliente> items;
        if (esPaginado)
        {
            items = await query.Skip((page!.Value - 1) * pageSize!.Value)
                            .Take(pageSize.Value)
                            .ToListAsync();
        }
        else
        {
            items = await query.OrderBy(i=>i.RazonSocial).ToListAsync();
        }
        
        return new PagedResult<Cliente>
        {
            Items = items,
            TotalRecords = totalItems,
            Page =  page ?? 1,
            PageSize = pageSize ?? totalItems
        };
    }

    public async Task<Cliente?> GetById(int idCliente)
    {
        return await _context.Clientes
                            .Include(c => c.CondicionFiscal)
                            .FirstOrDefaultAsync(c => c.Id == idCliente);
    }

    public async Task<List<CondicionFiscal>> GetCondicionFiscalsAsync()
    {
        return await _context.CondicionesFiscales.ToListAsync();
    }

    public void Add(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
    }

    public void Update(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
    }
}