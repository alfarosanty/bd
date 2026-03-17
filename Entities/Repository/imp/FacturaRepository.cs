using Microsoft.EntityFrameworkCore;
using BlumeAPI.Data.Entities;
using BlumeAPI.Repository;
public class FacturaRepository : IFacturaRepository
{
private readonly IDbConnectionFactory _factory;
private readonly AppDbContext _context;

public FacturaRepository(AppDbContext context, IDbConnectionFactory factory)
{
    _context = context;
    _factory = factory;
}    

    // ORM METHODS
public async Task<FacturaEntity?> GetByIdAsync(int idFactura)
{
    // Usamos el nombre exacto de la propiedad en la Entity: ArticulosFactura
    var query = _context.Facturas
        .AsNoTracking()
        .Include(f => f.Cliente)
        .Include(f => f.Articulos) 
            .ThenInclude(af => af.Articulo)
                .ThenInclude(a => a.Color)
        .Include(f => f.Articulos)
            .ThenInclude(af => af.Articulo)
                .ThenInclude(a => a.Medida)
        .Include(f => f.Articulos)
            .ThenInclude(af => af.Articulo)
                .ThenInclude(a => a.SubFamilia)
        .Where(f => f.IdFactura == idFactura);

    // Para ver el SQL en la consola de debug
    var sql = query.ToQueryString(); 

    return await query.FirstOrDefaultAsync();
}

}
