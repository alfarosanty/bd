using BlumeApi.Models;
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
        context.Presupuestos.Add(presupuesto);
        await context.SaveChangesAsync();

        return presupuesto.Id;
    }
}