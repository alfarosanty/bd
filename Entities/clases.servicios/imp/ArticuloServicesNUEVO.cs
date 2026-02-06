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
    // 1️⃣ Traemos el artículo principal con ArticuloPrecio (ORM)
    var entity = await _articuloRepository.GetByIdAsync(idArticulo);

    if (entity == null)
        return null;

    // 2️⃣ Traemos los objetos "chicos" desde sus repositorios
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

    if (entity.IdArticuloPrecio.HasValue && entity.IdArticuloPrecio.Value > 0)
    {
        var precioEntity = await _articuloRepository.GetArticuloPrecioByIdAsync(entity.IdArticuloPrecio.Value);

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



    // 4️⃣ Ejecutamos las tareas en paralelo
    await Task.WhenAll(colorTask, medidaTask, subFamiliaTask);

    // 5️⃣ Armamos el Articulo final
    return new Articulo
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
}