using BlumeApi.Models;
using BlumeAPI.Models;
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class PresupuestoRepository : IPresupuestoRepository{
     private readonly AppDbContext _context;

    public PresupuestoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Presupuesto?> GetPresupuesto(int idPresupuesto)
    {
        return await _context.Presupuestos

            .Include(p => p.Articulos)
                .ThenInclude(ap => ap.Articulo)

            .Include(p => p.Articulos)
                .ThenInclude(ap => ap.Articulo.Color)

            .Include(p => p.Articulos)
                .ThenInclude(ap => ap.Articulo.Medida)

            .Include(p => p.Articulos)
                .ThenInclude(ap => ap.Articulo.SubFamilia)

            .Include(p => p.Articulos)
                .ThenInclude(ap => ap.Articulo.ArticuloPrecio)

            .FirstOrDefaultAsync(p => p.Id == idPresupuesto);
    }

        public async Task<int> CrearPresupuestoAsync(Presupuesto presupuesto)
    {
        _context.Presupuestos.Add(presupuesto);
        await _context.SaveChangesAsync();

        return presupuesto.Id;
    }

    public async Task<bool> ActualizarPresupuestoAsync(Presupuesto presupuesto)
{
    _context.Presupuestos.Update(presupuesto);
    return await _context.SaveChangesAsync() > 0;
}

    public Task<List<Presupuesto>> GetPresupuestoByCliente(int clienteId)
    {
        return Task.FromResult(_context.Set<Presupuesto>()
                       .Where(p => p.IdCliente == clienteId)
                       .ToList());
    }

    public Task<List<EstadoPresupuesto>> getEstadosPresupuesto()
    {
        return Task.FromResult(_context.Set<EstadoPresupuesto>()
                       .ToList());
    }

public Task<List<ArticuloPresupuesto>> articulosPresupuestados(int idArticuloPrecio, DateTime fechaInicio, DateTime fechaFin)
{
    var query = from ap in _context.ArticulosPresupuesto
                join p in _context.Presupuestos on ap.IdPresupuesto equals p.Id
                where p != null
                      && ap.Articulo != null 
                      && ap.Articulo.ArticuloPrecio != null 
                      && ap.Articulo.ArticuloPrecio.Id == idArticuloPrecio
                      && p.Fecha >= fechaInicio
                      && p.Fecha <= fechaFin
                select ap;

    return Task.FromResult(query.ToList());
}

public async Task AgregarArticuloPresupuestoAsync(ArticuloPresupuesto articulo)
{
    _context.ArticulosPresupuesto.Add(articulo);
    await _context.SaveChangesAsync();

}
}