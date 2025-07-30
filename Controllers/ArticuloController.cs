using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticuloController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<ArticuloController> _logger;

    public ArticuloController(ILogger<ArticuloController> logger)
    {
        _logger = logger;
    }

// ARTICULO

     [HttpGet("GetArticulos")]
    public IEnumerable<Articulo> GetArticulos()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().getAll(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }





 [HttpGet("ByFamilias/{familia}")]
    public IEnumerable<Articulo> GetArticulosByFamilia(string familia)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().GetArticuloByFamiliaMedida(familia, null, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }

    [HttpGet("ByFamiliaMedida/{familia}/{medida}")]
    public IEnumerable<Articulo> GetArticulosByFamilia(string familia, string medida)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().GetArticuloByFamiliaMedida(familia, medida, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }


[HttpPost("crearArticulos")]
public List<int> crearArticulos(Articulo[] articulos)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    ArticuloServices articuloService = new ArticuloServices();
    List<int> idsGenerados = articuloService.crearArticulos(articulos, npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);
    return idsGenerados;
}

    [HttpPost("ConsultarMedidasNecesarias")]
    public ConsultaMedida[] ConsultarMedidasNecesarias([FromBody] ArticuloPresupuesto[] articulos)
     {
         ArticuloServices articuloService = new ArticuloServices();

        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();


         var resultado = articuloService.ConsultarMedidasNecesarias(articulos, npgsqlConnection);

         return resultado.ToArray(); // convertís List<ConsultaMedida> a ConsultaMedida[]
      }

        [HttpGet("{idArticuloPrecio}/cantidades-taller-corte")]
        public ConsultaTallerCortePorCodigo[] ConsultarCantidadesTallerCorte(int idArticuloPrecio)

     {
         ArticuloServices articuloService = new ArticuloServices();
         CConexion con =  new CConexion();
         Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();


         var resultado = articuloService.ConsultarCantidadesTallerCorte(idArticuloPrecio,npgsqlConnection);

         return resultado.ToArray();
      }

        [HttpGet("cantidades-taller-corte")]
        public ConsultaTallerCortePorCodigo[] ConsultarTodosArticulosConCantidades()

     {
         ArticuloServices articuloService = new ArticuloServices();
         CConexion con =  new CConexion();
         Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();


         var resultado = articuloService.ConsultarTodosArticulosCantidadesTallerCorte(npgsqlConnection);

         return resultado.ToArray();
      }

// ARTICULO PRECIO

    [HttpGet("GetArticulosPrecio")]
    public IEnumerable<ArticuloPrecio> GetArticuloPrecio()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<ArticuloPrecio> articulosPrecio = new ArticuloServices().GetArticuloPrecio(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulosPrecio;
    }


[HttpGet("ByArticuloPrecio/{articuloPrecio}")]
public IEnumerable<Articulo> GetArticulosByArticuloPrecioId(int articuloPrecio, [FromQuery] bool? habilitados = null)
{
    CConexion con = new CConexion();
    using (var npgsqlConnection = con.establecerConexion())  // <-- Mejor usar using
    {
        List<Articulo> articulos = new ArticuloServices().GetArticulosByArticuloPrecioId(articuloPrecio, habilitados ?? false, npgsqlConnection);
        // No necesitás llamar a cerrarConexion si usás "using"
        return articulos;
    }
}

    [HttpPost("CrearArticulosPrecios")]
    public List<int> CrearArticulosPrecios(ArticuloPrecio[] articuloPrecios)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<int> articulosPrecioId = new ArticuloServices().CrearArticulosPrecios(articuloPrecios, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulosPrecioId;
    }

    [HttpPost("ActualizarArticulosPrecios")]
    public List<int> ActualizarArticulosPrecios(ArticuloPrecio[] articuloPrecios)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<int> articulosPrecioId = new ArticuloServices().ActualizarArticulosPrecios(articuloPrecios, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulosPrecioId;
    }

}