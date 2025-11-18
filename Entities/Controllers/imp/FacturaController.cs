using Microsoft.AspNetCore.Mvc;
using Npgsql;
using BlumeAPI.Services;  // Cambia esto si tu servicio está en otro namespace
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Microsoft.OpenApi.Any;
using BlumeApi.Models;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class FacturaController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;
    private readonly IFacturaService iFacturaService;

    public FacturaController(ILogger<ClienteController> logger, IFacturaService facturaService)
    {
        iFacturaService = facturaService;
        _logger = logger;
    }

    [HttpPost("crear")]
    public async Task< int>  Crear(Factura factura){

        int id =  await iFacturaService.crear(factura);
        return id;
    }


/*
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
*/
[HttpGet("FacturacionXCliente")]
public async Task<ActionResult<List<RespuestaEstadistica>>> facturacionXCliente([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
{


    List<RespuestaEstadistica> listaDeRespuestasEstadisticas =  await iFacturaService.facturacionXCliente(fechaInicio, fechaFin);

    return listaDeRespuestasEstadisticas;
}



[HttpGet("GetPorFiltros")]
public async Task<ActionResult<List<Factura>>> getFacturaPorFiltro([FromQuery] int? idCliente, [FromQuery] string? tipoFactura,[FromQuery] int? puntoDeVenta,[FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
{
    List<Factura> listaDeFacturas = await iFacturaService.getFacturasPorFiltro(idCliente, tipoFactura, puntoDeVenta, fechaInicio, fechaFin);

    return listaDeFacturas;
}

}

