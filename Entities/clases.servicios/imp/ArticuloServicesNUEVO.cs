using BlumeAPI.Entities.clases.modelo;
using BlumeAPI.Repositories;
using BlumeAPI.Services;

class ArticuloServicesNUEVO : IArticuloService
{
    private readonly IArticuloRepository _articuloRepository;
    private readonly IColorRepository _colorRepository;
    private readonly IMedidaRepository _medidaRepository;
    private readonly ISubfamiliaRepository _subFamiliaRepository;
    
    public ArticuloServicesNUEVO(IArticuloRepository articuloRepository, IColorRepository colorRepository, IMedidaRepository medidaRepository, ISubfamiliaRepository subFamiliaRepository)
    {
        _articuloRepository = articuloRepository;
        _colorRepository = colorRepository;
        _medidaRepository = medidaRepository;
        _subFamiliaRepository = subFamiliaRepository;
    }
    
    // MÉTODOS NUEVOS

public async Task<Articulo?> GetArticulo(int idArticulo)
{
    var entity = await _articuloRepository
        .GetByIdAsync(idArticulo);

    if (entity == null)
        return null;

    return new Articulo
    {
        Id = entity.IdArticulo,
        Codigo = entity.Codigo,
        Descripcion = entity.Descripcion,
        Habilitado = entity.Habilitado,
        Stock = entity.Stock,

        Color = entity.Color == null ? null : new Color
        {
            Id = entity.Color.IdColor,
            Codigo = entity.Color.Codigo,
            Descripcion = entity.Color.Descripcion,
            ColorHexa = entity.Color.ColorHexa
        },

        Medida = entity.Medida == null ? null : new Medida
        {
            Id = entity.Medida.IdMedida,
            Codigo = entity.Medida.Codigo,
            Descripcion = entity.Medida.Descripcion
        },

        SubFamilia = entity.SubFamilia == null ? null : new SubFamilia
        {
            Id = entity.SubFamilia.IdSubFamilia,
            Codigo = entity.SubFamilia.Codigo,
            Descripcion = entity.SubFamilia.Descripcion
        },

        articuloPrecio = entity.ArticuloPrecio == null ? null : new ArticuloPrecio
        {
            Id = entity.ArticuloPrecio.IdArticuloPrecio,
            Codigo = entity.ArticuloPrecio.Codigo,
            Descripcion = entity.ArticuloPrecio.Descripcion,
            Precio1 = entity.ArticuloPrecio.Precio1,
            Precio2 = entity.ArticuloPrecio.Precio2,
            Precio3 = entity.ArticuloPrecio.Precio3,
            Relleno = entity.ArticuloPrecio.Relleno
        }
    };
}


        public async Task<List<CartaKardexDTO>> GetFacturadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta)
    {
        var entities = await _articuloRepository
            .GetFacturadosByArticulo(idArticulo, desde, hasta);

        var articulo = await GetArticulo(idArticulo);

        entities.ForEach(carta => carta.articulo = articulo);
    
        return entities.OrderBy(e=>e.Fecha)
        .ToList();
    }


    public async Task<List<CartaKardexDTO>> GetIngresadosByArticulo(int idArticulo, DateTime? desde, DateTime? hasta)
    {
        var entities = await _articuloRepository
            .GetIngresadosByArticulo(idArticulo, desde, hasta);

        var articulo = await GetArticulo(idArticulo);

        entities.ForEach(carta => carta.articulo = articulo);
        return entities.OrderBy(e=>e.Fecha)
        .ToList();
    }

    public async Task<List<CartaKardexDTO>> GetResumenKardex(int idArticulo, DateTime? desde, DateTime? hasta)
    {
        var facturados = await GetFacturadosByArticulo(idArticulo, desde, hasta);
        var ingresados = await GetIngresadosByArticulo(idArticulo, desde, hasta);

        var resumen = new List<CartaKardexDTO>();

        resumen.AddRange(facturados);
        resumen.AddRange(ingresados);

        return resumen.OrderBy(r => r.Fecha).ToList();
    }
/*
public async Task<List<Articulo>> GetArticulosByArticuloPrecioAsync(
    int articuloPrecioId,
    bool soloHabilitados)
{
    var entities = await _articuloRepository
        .GetByArticuloPrecioIdAsync(articuloPrecioId, soloHabilitados);

    var result = new List<Articulo>();

    foreach (var entity in entities)
    {
        var colorTask = entity.IdColor > 0
            ? _colorRepository.GetById(entity.IdColor)
            : Task.FromResult<Color?>(null);

        var medidaTask = entity.IdMedida > 0
            ? _medidaRepository.GetById(entity.IdMedida)
            : Task.FromResult<Medida?>(null);

        var subFamiliaTask = entity.IdSubFamilia > 0
            ? _subFamiliaRepository.GetById((int)entity.IdSubFamilia)
            : Task.FromResult<SubFamilia?>(null);

        ArticuloPrecio? articuloPrecio = null;

        if (entity.IdArticuloPrecio.HasValue && entity.IdArticuloPrecio > 0)
        {
            var precioEntity = await _articuloRepository
                .GetArticuloPrecioByIdAsync(entity.IdArticuloPrecio.Value);

            if (precioEntity != null)
            {
                articuloPrecio = new ArticuloPrecio
                {
                    Id = precioEntity.IdArticuloPrecio,
                    Codigo = precioEntity.Codigo,
                    Descripcion = precioEntity.Descripcion,
                    Precio1 = precioEntity.Precio1,
                    Precio2 = precioEntity.Precio2,
                    Precio3 = precioEntity.Precio3,
                    Relleno = precioEntity.Relleno
                };
            }
        }

        await Task.WhenAll(colorTask, medidaTask, subFamiliaTask);

        result.Add(new Articulo
        {
            Id = entity.IdArticulo,
            Codigo = entity.Codigo,
            Descripcion = entity.Descripcion,
            Habilitado = entity.Habilitado,
            Stock = entity.Stock,
            Color = colorTask.Result,
            Medida = medidaTask.Result,
            SubFamilia = subFamiliaTask.Result,
            articuloPrecio = articuloPrecio
        });
    }

    return result;
}
*/
}