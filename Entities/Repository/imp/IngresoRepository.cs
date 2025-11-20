using BlumeApi.Models;
using Entities.Repository;
using Microsoft.EntityFrameworkCore;

public class IngresoRepository : IIngresoRepository
{

private readonly AppDbContext _context;
    public IngresoRepository(AppDbContext context)
    {
        _context = context;

    }

    public async Task<int> CrearAsync(Ingreso ingreso)
    {
        _context.Ingresos.Add(ingreso);
        await  _context.SaveChangesAsync();
        return ingreso.Id;
    }

    public async Task<int> actualizarAsync(Ingreso ingreso)
    {
        _context.Ingresos.Update(ingreso);
        await _context.SaveChangesAsync();
        return ingreso.Id;
    }

    public async Task<List<int>> CrearDetallesIngresoPedidoProduccionAsync(List<PedidoProduccionIngresoDetalle> detalles)
    {
        List<int> idsCreados = new List<int>();

        foreach (var detalle in detalles)
        {
            _context.PedidosProduccionIngresoDetalles.Add(detalle);
            await _context.SaveChangesAsync();
            idsCreados.Add(detalle.Id);
        }

        return idsCreados;
    }

    public async Task<List<Ingreso>> GetIngresoByTallerAsync(int idTaller)
    {
        return await _context.Ingresos
            .Where(i => i.IdTaller == idTaller)
            .ToListAsync();
    }

    public async Task<Ingreso> GetIngresoAsync(int idIngreso)
    {
        return await _context.Ingresos
            .FirstOrDefaultAsync(i => i.Id == (int)idIngreso);
    }

    public async Task<List<PedidoProduccionIngresoDetalle>> GetDetallesPPIAsync(int idIngreso)
    {
        return await _context.PedidosProduccionIngresoDetalles
            .Where(d => d.IdIngreso == idIngreso)
            .ToListAsync();
    }

    public async Task<int> BorrarIngresoAsync(Ingreso ingreso)
    {
        _context.Ingresos.Remove(ingreso);
        await _context.SaveChangesAsync();
        return ingreso.Id;
    }

    public async Task CrearIngresoArticuloAsync(ArticuloIngreso ingresoArticulo)
    {
        _context.ArticulosIngreso.Add(ingresoArticulo);
        await _context.SaveChangesAsync();
    }

}