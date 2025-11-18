using BlumeAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ArticuloPrecioRepository : IArticuloPrecioRepository
{
    private readonly AppDbContext _context;

    public ArticuloPrecioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<int>> CrearArticulosPreciosAsync(ArticuloPrecio[] articuloPrecios)
    {
        var idsGenerados = new List<int>();

        foreach (var precio in articuloPrecios)
        {
            var entidad = new ArticuloPrecio
            {
                Codigo = precio.Codigo,
                Descripcion = precio.Descripcion,
                Precio1 = precio.Precio1 ?? 0,
                Precio2 = precio.Precio2 ?? 0,
                Precio3 = precio.Precio3 ?? 0
            };

            _context.ArticulosPrecio.Add(entidad);
            await _context.SaveChangesAsync();

            idsGenerados.Add(entidad.Id);
        }

        return idsGenerados;
    }


        public async Task<List<int>> ActualizarArticulosPreciosAsync(ArticuloPrecio[] articuloPrecios)
    {
        var filasAfectadas = new List<int>();

        foreach (var input in articuloPrecios)
        {
            int afectadas = 0;

            // Buscar ArticuloPrecio por CODIGO
            var precio = await _context.ArticulosPrecio
                .FirstOrDefaultAsync(ap => ap.Codigo == input.Codigo);

            if (precio == null)
            {
                filasAfectadas.Add(0);
                continue;
            }

            // 1️⃣ Actualizar precios
            precio.Precio1 = input.Precio1 ?? 0;
            precio.Precio2 = input.Precio2 ?? 0;
            precio.Precio3 = input.Precio3 ?? 0;

            afectadas++;

            // 2️⃣ Verificar si cambió la descripción
            bool descripcionCambio =
                !string.Equals(precio.Descripcion ?? "",
                               input.Descripcion ?? "",
                               StringComparison.OrdinalIgnoreCase);

            if (descripcionCambio)
            {
                // Actualizar descripción del ArticuloPrecio
                precio.Descripcion = input.Descripcion;
                afectadas++;

                // Actualizar descripción en ARTICULO
                var articulos = await _context.Articulos
                    .Where(a => a.Codigo == input.Codigo)
                    .ToListAsync();

                foreach (var art in articulos)
                {
                    art.Descripcion = input.Descripcion;
                    afectadas++;
                }
            }

            await _context.SaveChangesAsync();
            filasAfectadas.Add(afectadas);
        }

        return filasAfectadas;
    }

        public async Task<Dictionary<int, ArticuloPrecio>> ObtenerPreciosPorIdsAsync(int[] ids, bool habilitados)
    {
        if (ids == null || ids.Length == 0)
            return new Dictionary<int, ArticuloPrecio>();

        var precios = await _context.ArticulosPrecio
            .Where(ap => ids.Contains(ap.Id))    // ← Esto reemplaza al ANY(@ids)
            .ToListAsync();

        return precios.ToDictionary(p => p.Id);
    }

     public async Task<EstadisticaArticuloDTO> GetArticuloPresupuestadoAsync(
        int idArticuloPrecio,
        DateTime? fechaDesde,
        DateTime? fechaHasta)
    {
        // Query base
        var query = _context.ArticulosPresupuesto
            .Include(ap => ap.Presupuesto)
            .Where(ap => ap.IdArticulo == idArticuloPrecio);

        // Fechas opcionales
        if (fechaDesde.HasValue)
            query = query.Where(ap => ap.Presupuesto.Fecha >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(ap => ap.Presupuesto.Fecha <= fechaHasta.Value);

        // Ejecutar en una sola consulta
        var datos = await query.ToListAsync();

        int cantidadTotal = datos.Sum(d => d.Cantidad);

        // Esto replica tu código, aunque no lo devolvés.
        DateTime? fechaUltimoPresupuesto = datos
            .Select(d => d.Presupuesto.Fecha)
            .OrderByDescending(f => f)
            .FirstOrDefault();

        // Obtener el artículo completo
        var articulo = await _context.Articulos
            .FirstOrDefaultAsync(a => a.Id == idArticuloPrecio);

        return new EstadisticaArticuloDTO
        {
            Articulo = articulo,
            CantidadPresupuestada = cantidadTotal,
        };
    }

    public async Task<List<ArticuloPrecio>> GetAllAsync()
    {
        return await _context.ArticulosPrecio
            .AsNoTracking()
            .ToListAsync();
    }

}
