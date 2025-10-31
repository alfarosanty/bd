using Microsoft.AspNetCore.Mvc;
using Npgsql;
using BlumeAPI.Services;  // Cambia esto si tu servicio está en otro namespace
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Microsoft.OpenApi.Any;

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



 [HttpPost("pruebaAFIP")]
    public async Task<ActionResult<FECAESolicitarResponse>> CrearAFIPAsync([FromBody] Factura factura)
    {
        try
        {
            // Conexión a BD
            CConexion con = new CConexion();
            using var npgsqlConnection = con.establecerConexion();

            // Autenticación AFIP
            AfipServices afipServices = new AfipServices();
            var loginTicket = await afipServices.AutenticacionAsync(verbose: true, npgsqlConnection);

            // Facturar en AFIP
            FacturaServices fs = new FacturaServices();
            var facturaAfipResponse = await fs.FacturarAsync(factura, loginTicket, Convert.ToInt64(20302367613));

            con.cerrarConexion(npgsqlConnection);

            return Ok(facturaAfipResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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

}

