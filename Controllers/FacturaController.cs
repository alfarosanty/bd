using Microsoft.AspNetCore.Mvc;
using Npgsql;
using BlumeAPI.Services;  // Cambia esto si tu servicio está en otro namespace

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class FacturaController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;

    public FacturaController(ILogger<ClienteController> logger)
    {
        _logger = logger;
    }

    [HttpPost("crear")]
    public int  Crear(Factura factura){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        FacturaServices  fs = new FacturaServices();
        int id =  fs.crear(factura, npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
        return id;
    }

[HttpGet("FacturacionXCliente")]
public ActionResult<List<RespuestaEstadistica>> facturacionXCliente([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    FacturaServices fs = new FacturaServices();

    // Llamás al servicio pasando las fechas recibidas
    List<RespuestaEstadistica> listaDeRespuestasEstadisticas = fs.facturacionXCliente(fechaInicio, fechaFin, npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);

    return listaDeRespuestasEstadisticas;
}



[HttpGet("GetPorFiltros")]
public ActionResult<List<Factura>> getFacturaPorFiltro([FromQuery] int? idCliente, [FromQuery] string? tipoFactura,[FromQuery] int? puntoDeVenta,[FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    FacturaServices fs = new FacturaServices();

    // Llamás al servicio pasando las fechas recibidas
    List<Factura> listaDeFacturas = fs.getFacturaPorFiltro(idCliente, tipoFactura, puntoDeVenta, fechaInicio, fechaFin, npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);

    return listaDeFacturas;
}
/*
[HttpGet("GetArticulos/{id}")]
public ActionResult<List<ArticuloFactura>> getArticulosXFactura(int idFactura)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    FacturaServices fs = new FacturaServices();

    // Llamás al servicio pasando las fechas recibidas
    List<ArticuloFactura> listaDeArticulosFactura = fs.getFacturaPorFiltro(idFactura, npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);

    return listaDeArticulosFactura;
}
*/
/*
    [HttpPost("Autenticar")]
    public IActionResult Autenticar([FromBody] AutenticacionDTO request)
    {
        try
        {
            var service = new FacturaServices();
            var auth = service.autenticar(
                request.Cuit,
                request.PfxPath,
                request.PfxPassword,
                request.Servicio
            );

            return Ok(new { Token = auth.Token, Sign = auth.Sign });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }*/
}

