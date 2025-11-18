using BlumeAPI.Models;
using Npgsql;


public class ArticuloService : IArticuloService
{
    private readonly IArticuloRepository iarticuloRepository;
    private readonly IArticuloPrecioRepository iarticuloPrecioRepository;


    public ArticuloService(IArticuloRepository _iarticuloRepository, IArticuloPrecioRepository _iarticuloPrecioRepository)
    {
        iarticuloRepository = _iarticuloRepository;
        iarticuloPrecioRepository = _iarticuloPrecioRepository;
    }
    public async Task<List<Articulo>> GetAllAsync()
    {
        return await iarticuloRepository.GetAllAsync();
    
    }

    public async Task<Articulo?> GetArticuloAsync(int id)
    {
        return await iarticuloRepository.GetByIdAsync(id);
    }

    public async Task<List<Articulo>> GetArticulosByArticuloPrecioId(int articuloPrecioId, bool habilitados)
    {
        return await iarticuloRepository.GetArticulosByArticuloPrecioIdAsync( articuloPrecioId, habilitados);
    }

    public async Task<List<int>> CrearArticulosAsync(List<Articulo> articulos)
    {
        return await iarticuloRepository.CrearArticulosAsync(articulos);
    }

    public async Task<List<ConsultaMedida>> ConsultarMedidasNecesarias(
        ArticuloPresupuesto[] presupuestosArticulos)
    {
        var acumulador = new Dictionary<string, int>();

        foreach (var presuArt in presupuestosArticulos)
        {
            if (presuArt?.Articulo?.Medida == null)
                continue;

            string codigoMedida = presuArt.Articulo.Medida.Codigo;
            int cantidad = presuArt.Cantidad;

            // Si no tiene relleno, usar 1
            int relleno = presuArt.Articulo.ArticuloPrecio?.Relleno ?? 1;

            int cantidadFinal = cantidad * relleno;

            if (acumulador.ContainsKey(codigoMedida))
            {
                acumulador[codigoMedida] += cantidadFinal;
            }
            else
            {
                acumulador[codigoMedida] = cantidadFinal;
            }
        }

        // Convertir diccionario → lista
        var lista = acumulador
            .Select(kv => new ConsultaMedida
            {
                Medida = kv.Key,
                Cantidad = kv.Value
            })
            .ToList();

        return await Task.FromResult(lista);
    }


public async Task<List<ConsultaTallerCortePorCodigo>> ConsultarCantidadesTallerCorte(string codigo)
{
    // Llamadas async al repositorio
    var stock = await iarticuloRepository.ConsultarStockAsync(codigo);
    var enCorte = await iarticuloRepository.ConsultarEnCorteAsync(codigo);
    var enTaller = await iarticuloRepository.ConsultarEnTallerAsync(codigo);
    var separados = await iarticuloRepository.ConsultarSeparadosAsync(codigo);

    // Mapear a ConsultaTallerCorte
    var resultado = stock.Select(s =>
    {
        var articulo = s.articulo;

        var corte = enCorte.FirstOrDefault(x => x.articulo.Id == articulo.Id);
        var taller = enTaller.FirstOrDefault(x => x.articulo.Id == articulo.Id);
        var separado = separados.FirstOrDefault(x => x.articulo.Id == articulo.Id);

        return new ConsultaTallerCorte
        {
            articulo = articulo,
            StockUnitario = s.StockUnitario,
            CantidadEnCorteUnitario = corte?.CantidadEnCorteUnitario ?? 0,
            CantidadEnTallerUnitario = taller?.CantidadEnTallerUnitario ?? 0,
            CantidadSeparadoUnitario = separado?.CantidadSeparadoUnitario ?? 0,
            CantidadEstanteriaUnitario = s.StockUnitario - (separado?.CantidadSeparadoUnitario ?? 0)
        };
    }).ToList();

    // Si se pidió un código específico
    if (!string.IsNullOrEmpty(codigo))
    {
        var agrupado = new ConsultaTallerCortePorCodigo
        {
            Codigo = codigo,
            StockTotal = resultado.Sum(x => x.StockUnitario),
            CantidadEnCorteTotal = resultado.Sum(x => x.CantidadEnCorteUnitario),
            CantidadEnTallerTotal = resultado.Sum(x => x.CantidadEnTallerUnitario),
            CantidadSeparadoTotal = resultado.Sum(x => x.CantidadSeparadoUnitario),
            CantidadEstanteriaTotal = resultado.Sum(x => x.CantidadEstanteriaUnitario),
            Consultas = resultado
        };

        return new List<ConsultaTallerCortePorCodigo> { agrupado };
    }

    // Agrupar por cada código de artículo
    var agrupadoPorCodigo = resultado
        .GroupBy(r => r.articulo.Codigo)
        .Select(g => new ConsultaTallerCortePorCodigo
        {
            Codigo = g.Key,
            StockTotal = g.Sum(x => x.StockUnitario),
            CantidadEnCorteTotal = g.Sum(x => x.CantidadEnCorteUnitario),
            CantidadEnTallerTotal = g.Sum(x => x.CantidadEnTallerUnitario),
            CantidadSeparadoTotal = g.Sum(x => x.CantidadSeparadoUnitario),
            CantidadEstanteriaTotal = g.Sum(x => x.CantidadEstanteriaUnitario),
            Consultas = g.ToList()
        })
        .ToList();

    return agrupadoPorCodigo;
}


    public async Task<List<ConsultaTallerCortePorCodigo>> ConsultarTodosArticulosCantidadesTallerCorte()
    {
        return await ConsultarCantidadesTallerCorte("");
    }

    public async Task<int> ActualizarStockAsync(ActualizacionStockInutDTO[] articulos)
    {
        return await iarticuloRepository.ActualizarStockAsync(articulos);
    }


// ================================== ARTICULO PRECIO ==================================

    public async Task<List<ArticuloPrecio>> GetArticulosPrecioAsync()
    {
        return await iarticuloPrecioRepository.GetAllAsync();
    }

        public async Task<List<int>> CrearArticulosPreciosAsync(ArticuloPrecio[] articuloPrecios)
    {
        return await iarticuloPrecioRepository.CrearArticulosPreciosAsync(articuloPrecios);
    }

        public async Task<List<int>> ActualizarArticulosPreciosAsync(ArticuloPrecio[] articuloPrecios)
    {
        return await iarticuloPrecioRepository.ActualizarArticulosPreciosAsync(articuloPrecios);
    }

    public Task<EstadisticaArticuloDTO> GetArticuloPresupuestadoAsync(int idArticuloPrecio, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        return iarticuloPrecioRepository.GetArticuloPresupuestadoAsync(idArticuloPrecio, fechaDesde, fechaHasta);
    }
}


    

