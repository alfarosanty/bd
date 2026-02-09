/*public class FacturaRepository : IFacturaRepository
{
    private readonly IDbConnectionFactory _factory;
    private readonly AppDbContext _context;

    public FacturaRepository(AppDbContext context, IDbConnectionFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public async Task<FacturaEntity?> GetByIdAsync(int idFactura)
    {
        return await _context.Facturas
            .Include(f => f.Cliente)
            .Include(f => f.Detalles)
                .ThenInclude(d => d.Articulo)
            .FirstOrDefaultAsync(f => f.Id == idFactura);
    }

    public async Task<List<FacturaEntity>> GetFacturasByClienteAsync(int idCliente, DateTime? desde, DateTime? hasta)
    {
        var query = _context.Facturas
            .Include(f => f.Cliente)
            .Include(f => f.Detalles)
                .ThenInclude(d => d.Articulo)
            .Where(f => f.IdCliente == idCliente);

        if (desde.HasValue)
            query = query.Where(f => f.Fecha >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(f => f.Fecha <= hasta.Value);

        return await query.ToListAsync();
    }

    public async Task<int> createFacturaAsync(FacturaEntity factura)
    {
        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync();
        return factura.Id;
    }

    public async Task<FacturaDTO?> GetFacturaByIdAsync(int idFactura)
    {
        // Implementación DAPPER para obtener la factura como DTO
        // ...
        return null; // Placeholder
    }
}
*/