using BlumeApi.Models;
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class PedidoProduccionRepository:IPedidoProduccionRepository
{

    private readonly AppDbContext context;

    public PedidoProduccionRepository(AppDbContext _context)
    {
        context = _context;
    }

    public async Task<int?> CrearAsync(PedidoProduccion pedidoProduccion)
    {
        context.PedidosProduccion.Add(pedidoProduccion);
        await context.SaveChangesAsync();
        return pedidoProduccion.Id;
    }

    public async Task CrearArticuloAsync(PedidoProduccionArticulo articuloProduccion)
    {
        context.PedidosProduccionArticulos.Add(articuloProduccion);
        await context.SaveChangesAsync();
    }

    public async Task<int> ActualizarAsync(PedidoProduccion pedidoProduccion)
    {
        context.PedidosProduccion.Update(pedidoProduccion);
        await context.SaveChangesAsync();
        return pedidoProduccion.Id;
    }

public async Task<PedidoProduccion> GetPedidoProduccionAsync(int id)
{
    return await context.PedidosProduccion
        .Include(p => p.Articulos)
        .Include(p => p.Taller)
        .FirstOrDefaultAsync(p => p.Id == id);
}

    public async Task<int> EliminarPedidoProduccionAsync(int idPedidoProduccio)
    {
        var pedido = await context.PedidosProduccion.FindAsync(idPedidoProduccio);
        if (pedido != null)
        {
            context.PedidosProduccion.Remove(pedido);
            await context.SaveChangesAsync();
            return idPedidoProduccio;
        }
        return -1; // Indica que no se encontró el pedido
    }

    public async Task<List<PedidoProduccion>> GetPedidoProduccionByTallerAsync(int idTaller)
    {
        return await context.PedidosProduccion
            .Where(p => p.IdTaller == idTaller)
            .ToListAsync();
    }

    public async Task<List<EstadoPedidoProduccion>> GetEstadosPedidoProduccionAsync()
    {
        return await context.EstadosPedidoProduccion.ToListAsync();
    }

}