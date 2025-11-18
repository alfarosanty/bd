using BlumeAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ArticuloRepository : IArticuloRepository
{
        private readonly AppDbContext _context;

    public ArticuloRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<ConsultaMedida>> ConsultarMedidasNecesarias(ArticuloPresupuesto[] articulosPresupuesto)
    {
        throw new NotImplementedException();
    }
public async Task<List<ConsultaTallerCorte>> ConsultarEnCorteAsync(string? codigo)
{
    var query = _context.PedidosProduccion
        .Where(pp => pp.IdEstadoPedidoProduccion == 2)
        .SelectMany(pp => pp.Articulos)
        .Where(pa => pa.Articulo != null &&
                     pa.Articulo.Color != null)
        .AsQueryable();

    // Filtro opcional por código
    if (!string.IsNullOrWhiteSpace(codigo))
        query = query.Where(pa => pa.Codigo == codigo);

    var resultado = await query
        .GroupBy(pa => new
        {
            IdArticulo = pa.Articulo.Id,
            CodigoArticulo = pa.Codigo,
            IdColor = pa.Articulo.Color.Id,
            CodigoColor = pa.Articulo.Color.Codigo
        })
        .Select(g => new ConsultaTallerCorte
        {
            articulo = new Articulo
            {
                Id = g.Key.IdArticulo,
                Codigo = g.Key.CodigoArticulo,
                Color = new Color
                {
                    Id = g.Key.IdColor,
                    Codigo = g.Key.CodigoColor
                }
            },
            CantidadEnCorteUnitario = g.Sum(x => x.Cantidad),
            CantidadEnTallerUnitario = 0,
            CantidadSeparadoUnitario = 0,
            StockUnitario = 0,
            CantidadEstanteriaUnitario = 0
        })
        .OrderBy(x => x.articulo.Id)
        .ToListAsync();

    return resultado;
}
public async Task<List<ConsultaTallerCorte>> ConsultarEnTallerAsync(string? codigo)
{
    var query = _context.PedidosProduccion
        .Where(pp => pp.IdEstadoPedidoProduccion == 3)
        .SelectMany(pp => pp.Articulos)
        .Where(pa => pa.Articulo != null &&
                     pa.Articulo.Color != null)
        .AsQueryable();

    // Filtro opcional
    if (!string.IsNullOrWhiteSpace(codigo))
        query = query.Where(pa => pa.Codigo == codigo);

    var resultado = await query
        .GroupBy(pa => new
        {
            IdArticulo = pa.Articulo.Id,
            CodigoArticulo = pa.Codigo,
            IdColor = pa.Articulo.Color.Id,
            CodigoColor = pa.Articulo.Color.Codigo
        })
        .Select(g => new ConsultaTallerCorte
        {
            articulo = new Articulo
            {
                Id = g.Key.IdArticulo,
                Codigo = g.Key.CodigoArticulo,
                Color = new Color
                {
                    Id = g.Key.IdColor,
                    Codigo = g.Key.CodigoColor
                }
            },
            CantidadEnTallerUnitario = g.Sum(x => x.Cantidad),
            CantidadEnCorteUnitario = 0,
            CantidadSeparadoUnitario = 0,
            StockUnitario = 0,
            CantidadEstanteriaUnitario = 0
        })
        .OrderBy(x => x.articulo.Id)
        .ToListAsync();

    return resultado;
}



public async Task<List<ConsultaTallerCorte>> ConsultarSeparadosAsync(string? codigo)
{
var query = _context.Presupuestos
    .Where(p => p.IdEstadoPresupuesto == 2)
    .SelectMany(p => p.Articulos)
    .Where(ap => ap.HayStock == true &&
                 ap.Articulo != null &&
                 ap.Articulo.Color != null)
    .AsQueryable();


    // Filtro opcional por código
    if (!string.IsNullOrWhiteSpace(codigo))
        query = query.Where(ap => ap.Codigo == codigo);

    var resultado = await query
        .GroupBy(ap => new
        {
            IdArticulo = ap.Articulo!.Id,
            CodigoArticulo = ap.Codigo,
            IdColor = ap.Articulo!.Color!.Id,
            CodigoColor = ap.Articulo!.Color!.Codigo
        })
        .Select(g => new ConsultaTallerCorte
        {
            articulo = new Articulo
            {
                Id = g.Key.IdArticulo,
                Codigo = g.Key.CodigoArticulo,
                Color = new Color
                {
                    Id = g.Key.IdColor,
                    Codigo = g.Key.CodigoColor
                }
            },
            CantidadSeparadoUnitario = g.Sum(x => x.Cantidad),
            CantidadEnCorteUnitario = 0,
            CantidadEnTallerUnitario = 0,
            StockUnitario = 0,
            CantidadEstanteriaUnitario = 0
        })
        .OrderBy(x => x.articulo.Id)
        .ToListAsync();

    return resultado;
}


public async Task<List<ConsultaTallerCorte>> ConsultarStockAsync(string? codigo)
{
    var query = _context.Articulos
        .Include(a => a.Color) // JOIN COLOR
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(codigo))
        query = query.Where(a => a.Codigo == codigo);

    return await query
        .Select(a => new ConsultaTallerCorte
        {
            articulo = new Articulo
            {
                Id = a.Id,
                Codigo = a.Codigo,
                Descripcion = a.Descripcion,
                Color = new Color
                {
                    Id = a.Color.Id,
                    Codigo = a.Color.Codigo,
                    Descripcion = a.Color.Descripcion,
                    ColorHexa = a.Color.ColorHexa
                }
            },
            StockUnitario = (int)a.Stock,
            CantidadEnCorteUnitario = 0,
            CantidadEnTallerUnitario = 0,
            CantidadSeparadoUnitario = 0,
            CantidadEstanteriaUnitario = 0
        })
        .OrderBy(x => x.articulo.Id)
        .ToListAsync();
}


public async Task<List<int>> CrearArticulosAsync(List<Articulo> articulos)
    {
        var ids = new List<int>();

        foreach (var articulo in articulos)
        {
            if (articulo.Nuevo == true)
            {
                // EF lo toma como nuevo
                articulo.Id = 0; 
                articulo.Stock = 0;

                _context.Articulos.Add(articulo);
                await _context.SaveChangesAsync(); // genera ID

                ids.Add(articulo.Id);
            }
            else
            {
                // UPDATE
                var existente = await _context.Articulos
                    .FirstOrDefaultAsync(a => a.Id == articulo.Id);

                if (existente != null)
                {
                    existente.Habilitado = articulo.Habilitado;
                    await _context.SaveChangesAsync();
                }

                ids.Add(articulo.Id);
            }
        }

        return ids;
    }

public async Task<int> ActualizarStockAsync(ActualizacionStockInutDTO[] articulos)
    {
        int cantidadTotal = 0;

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in articulos)
            {
                var articulo = await _context.Articulos
                    .FirstOrDefaultAsync(a => a.Id == item.IdArticulo);

                if (articulo != null)
                {
                    articulo.Stock = item.CantidadStock;
                    cantidadTotal++;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return cantidadTotal;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Articulo>> GetArticulosByArticuloPrecioIdAsync(int articuloPrecioId, bool habilitados)
    {
        var query = _context.Articulos
            .Include(a => a.Medida)
            .Include(a => a.SubFamilia)
            .Include(a => a.Color)
            .Include(a => a.ArticuloPrecio)
            .Where(a => a.ArticuloPrecio.Id == articuloPrecioId);

        if (habilitados)
            query = query.Where(a => a.Habilitado == true);

        return await query
            .OrderBy(a => a.Id)
            .ToListAsync();
    }

public async Task<List<Articulo>> GetAllAsync()
{
    return await _context.Articulos
        .Include(a => a.Color)
        .Include(a => a.Medida)
        .Include(a => a.SubFamilia)
        .Include(a => a.ArticuloPrecio)
        .OrderBy(a => a.Id)
        .ToListAsync();
}


public async Task<Articulo> GetByIdAsync(int id)
{
    return await _context.Articulos
        .Include(a => a.Color)
        .Include(a => a.Medida)
        .Include(a => a.SubFamilia)
        .Include(a => a.ArticuloPrecio)
        .FirstOrDefaultAsync(a => a.Id == id);
}

}