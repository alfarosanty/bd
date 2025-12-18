using Microsoft.AspNetCore.Mvc;
using Npgsql;
using BlumeAPI.Services;  // Cambia esto si tu servicio está en otro namespace
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Microsoft.OpenApi.Any;
using BlumeAPI.servicios;

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
    public ActionResult  Crear(Factura factura){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        FacturaServices  fs = new FacturaServices();
        int id =  fs.crear(factura, npgsqlConnection);
        factura.Id = id;
         con.cerrarConexion(npgsqlConnection);
        return Ok(new
        {
            idInterno = id,
            respuestaCompleta = factura
        });    }

    [HttpPost("crearConAFIP")]
public async Task<ActionResult> CrearConAFIPAsync([FromBody] Factura factura)
{
    Npgsql.NpgsqlConnection? npgsqlConnection = null;

    try
    {
        // 1) Establecemos conexiones

        CConexion con = new CConexion();
        npgsqlConnection = con.establecerConexion();

        FacturaServices fs = new FacturaServices();

        // 2) Facturo de manera interna

        int idInterno = fs.crear(factura, npgsqlConnection);
        
        // 3) Facturo en AFIP

        AfipServices afipServices = new AfipServices();
        var loginTicket = await afipServices.AutenticacionAsync(
            verbose: false,
            npgsqlConnection
        );

        factura.Id = idInterno;

        var respuestaAfip = await fs.FacturarAsync(
            factura,
            loginTicket,
            Convert.ToInt64(20302367613)
        );

        // 4) Guardar datos del CAE en DB
        fs.ActualizarDatosAFIP(
            facturaId: idInterno,
            respuestaAfip,
            npgsqlConnection
        );

        con.cerrarConexion(npgsqlConnection);

        return Ok(new
        {
            idInterno,
            respuestaCompleta = respuestaAfip
        });
    }
    catch (Exception ex)
    {
        if (npgsqlConnection != null)
        {
            CConexion con = new CConexion();
            con.cerrarConexion(npgsqlConnection);
        }

        return BadRequest(new
        {
            error = ex.Message,
            stack = ex.StackTrace
        });
    }
}




 [HttpPost("pruebaAFIP")]
    public async Task<ActionResult<string?>> CrearAFIPAsync([FromBody] Factura factura)
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
/*
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GenerarPdf(int id)
    {
            // Conexión a BD
            CConexion con = new CConexion();
            using var npgsqlConnection = con.establecerConexion();

            // Autenticación AFIP
            AfipServices afipServices = new AfipServices();
            FacturaServices facturaServices = new FacturaServices();
            PdfService pdfService = new PdfService();
            TemplateService templateService = new TemplateService();
            FacturaBuilder _facturaBuilder = new FacturaBuilder(templateService, pdfService);

            var factura = facturaServices.GetFactura(id, npgsqlConnection);
        if (factura == null)
            return NotFound("Factura no encontrada.");

        var pdfDocument = _facturaBuilder.Build(factura);

        var pdfBytes = await _facturaBuilder.Build(factura);

        return File(pdfBytes, "application/pdf", $"Factura_{id}.pdf");
    }
*/

[HttpGet("{id}")]
public async Task<IActionResult> GetFactura(int id)
{
    CConexion con = new CConexion();

    using var npgsqlConnection = con.establecerConexion();

    FacturaServices facturaServices = new FacturaServices();

    var factura = facturaServices.GetFactura(id, npgsqlConnection);

    if (factura == null)
        return NotFound("Factura no encontrada.");

    return Ok(factura);
}


[HttpGet("{id}/pdf-test")]
public IActionResult PdfTest(int id)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    var srv = new FastReportService();
    FacturaServices facturaServices = new FacturaServices();
    Factura factura = facturaServices.GetFactura(id, npgsqlConnection);
    var pdfBytes = srv.CrearPdf(factura);

    return File(pdfBytes, "application/pdf", "Factura_Test.pdf");
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

