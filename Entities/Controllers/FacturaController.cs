using Microsoft.AspNetCore.Mvc;
using Npgsql;
using BlumeAPI.Services;  // Cambia esto si tu servicio está en otro namespace
using BlumeAPI.Entities;
using Microsoft.Extensions.Options;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class FacturaController : ControllerBase
{

    private readonly IARCAService _arcaService;
    private readonly IFacturaService _facturaService;
    private readonly IDbConnectionFactory _factory;
    private readonly AfipWsfeClient _afipClient;
    private readonly AfipSettings _afipSettings;
    private readonly AppDbContext _context;



    public FacturaController(IARCAService arcaService, 
                             IFacturaService facturaService, 
                             IDbConnectionFactory factory, 
                             AfipWsfeClient afipClient,
                             IOptions<AfipSettings> afipSettings,
                            AppDbContext context
                             )
    {
        _arcaService = arcaService;
        _facturaService = facturaService;
        _factory = factory;
        _afipClient = afipClient;
        _afipSettings = afipSettings.Value;
        _context = context;
    }

    [HttpPost("crear")]
    public ActionResult  Crear(Factura factura){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        FacturaServices  fs = new FacturaServices(_afipClient);
        int id =  fs.crear(factura, npgsqlConnection);
        factura.Id = id;
         con.cerrarConexion(npgsqlConnection);
        return Ok(new
        {
            idInterno = id,
            respuestaCompleta = factura
        });    }


[HttpGet("GetByCliente/{idCliente}")]
public IActionResult GetFacturasPorCliente(
    int idCliente,
    [FromQuery] DateTime? desde,
    [FromQuery] DateTime? hasta)
{
    if (idCliente <= 0)
        return BadRequest("Id de cliente inválido");

    var con = new CConexion();
    NpgsqlConnection npgsqlConnection = null;

    try
    {
        npgsqlConnection = con.establecerConexion();

        var fs = new FacturaServices(_afipClient);
        var facturas = fs.GetFacturasByCliente(
            idCliente,
            desde,
            hasta,
            npgsqlConnection
        );

        if (facturas == null || facturas.Count == 0)
            return NoContent();

        return Ok(facturas);
    }
    catch (PostgresException ex)
    {
        return StatusCode(500, "Error de la Base de datos: " + ex.Message);
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Error interno del servidor" + ex.Message);
    }
    finally
    {
        if (npgsqlConnection != null)
            con.cerrarConexion(npgsqlConnection);
    }
}

[HttpGet("{id}/pdf")]
public IActionResult PdfFactura(int id)
{
    CConexion con = new CConexion();
    using var npgsqlConnection = con.establecerConexion();

    var srv = new FastReportService(_afipClient);
    var facturaServices = new FacturaServices(_afipClient);

    Factura factura = facturaServices.GetFactura(id, npgsqlConnection);

    var pdfOriginal = srv.CrearPdf(factura, "ORIGINAL");
    var pdfDuplicado = srv.CrearPdf(factura, "DUPLICADO");
    var pdfTriplicado = srv.CrearPdf(factura, "TRIPLICADO");

    var pdfFinal = PdfUtils.UnirPdfs(
        pdfOriginal,
        pdfDuplicado,
        pdfTriplicado
    );

    var fileName = $"Factura_{factura.TipoFactura}-{factura.Cliente.RazonSocial}.pdf";

    return File(pdfFinal, "application/pdf", fileName);
}



[HttpGet("FacturacionXCliente")]
public ActionResult<List<RespuestaEstadistica>> facturacionXCliente([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    FacturaServices fs = new FacturaServices(_afipClient);

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

    FacturaServices fs = new FacturaServices(_afipClient);

    // Llamás al servicio pasando las fechas recibidas
    List<Factura> listaDeFacturas = fs.getFacturaPorFiltro(idCliente, tipoFactura, puntoDeVenta, fechaInicio, fechaFin, npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);

    return listaDeFacturas;
}

    [HttpPost("crearConAFIP")]
    public async Task<ActionResult> CrearConAFIPAsync([FromBody] Factura factura)
    {
        if (factura == null)
            return BadRequest(new { error = "Factura inválida." });

        using var connection = new CConexion().establecerConexion();
        using var transaction = connection.BeginTransaction();

        try
        {
            var facturaService = new FacturaServices(_afipClient);
            

            // =========================
            // 1️⃣ Crear factura interna
            // =========================
            int idInterno = facturaService.crear(factura, connection);
            factura.Id = idInterno;

            // =========================
            // 2️⃣ Autenticación AFIP
            // =========================
            var servicioPadron = "wsfe";
            var loginTicket = await _arcaService.AutenticacionAsync(servicioPadron);

            if (loginTicket == null)
                throw new Exception("No se pudo autenticar contra AFIP.");

            // =========================
            // 3️⃣ Facturar en AFIP
            // =========================
            var respuestaAfip = await facturaService.FacturarWsfeAsync(
                factura,
                loginTicket,
                long.Parse(_afipSettings.Cuit)
            );

            if (!respuestaAfip.Aprobado)
            {
                var errores = string.Join(" | ", respuestaAfip.Errores);
                throw new Exception($"AFIP rechazó la factura: {errores}");
            }

            // =========================
            // 4️⃣ Guardar CAE
            // =========================
            facturaService.ActualizarDatosAFIP(
                idInterno,
                respuestaAfip,
                connection
            );

            // =========================
            // 5️⃣ Commit
            // =========================
            transaction.Commit();

            return Ok(new
            {
                idInterno,
                cae = respuestaAfip.Cae,
                vencimiento = respuestaAfip.CaeVencimiento,
                observaciones = respuestaAfip.Observaciones
            });
        }
        catch (Exception ex)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // si falla el rollback no queremos romper más
            }

            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        [FromQuery] bool? facturadoARCA,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
    if (page < 1 || pageSize < 1)
            return BadRequest("Los parámetros de paginación deben ser mayores a 0.");

        var result = await _facturaService.GetFacturasAsync(desde, hasta, facturadoARCA, page, pageSize);

        if (result == null || result.Items.Count == 0)
            return NoContent();

        return Ok(result);
    }

[HttpGet("cliente/{idCliente}")]
    public async Task<IActionResult> GetByCliente(
        [FromRoute] int idCliente,
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        [FromQuery] bool? facturadoARCA,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
        var result = await _facturaService.GetFacturasByClienteAsync(idCliente, desde, hasta, facturadoARCA, page, pageSize);

        if (result == null || result.Items.Count == 0)
            return NoContent();

        return Ok(result);
    }


[HttpGet("{id}")]
public async Task<IActionResult> GetFactura(int id)
{
    if (id <= 0)
        return BadRequest("Id de factura inválido");

    try
    {
        var factura = await _facturaService.GetByIdAsync(id);

        if (factura == null)
            return NotFound();

        return Ok(factura);
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Error interno del servidor: " + ex.Message);
    }
}

[HttpPost("{id}/NotaCredito")]
public async Task<ActionResult> CrearNotaDeCreditoAsync([FromBody] NotaDeCredito notaDeCredito)
{
    using var efTransaction = await _context.Database.BeginTransactionAsync();
    
    try
    {
        // 1️⃣ Crear NC interna
        var response = await _facturaService.CrearNotaCreditoAsync(notaDeCredito);
        notaDeCredito.Id = response.Id;

        // 2️⃣ Autenticación ARCA
        using var connection = (NpgsqlConnection)_factory.CreateConnection();
        connection.Open();

        var servicioPadron = "wsfe";
        var loginTicket = await _arcaService.AutenticacionAsync(servicioPadron);

        if (loginTicket == null)
            throw new Exception("No se pudo autenticar contra ARCA.");

        // 3️⃣ Autorizar en ARCA
        var respuestaAfip = await _facturaService.ValidarNotaCreditoWsfeAsync(
            notaDeCredito,
            loginTicket,
            long.Parse(_afipSettings.Cuit)
        );

        if (!respuestaAfip.Aprobado)
        {
            var errores = string.Join(" | ", respuestaAfip.Errores);
            throw new Exception($"ARCA rechazó la nota de crédito: {errores}");
        }

        // 4️⃣ Guardar CAE
        Console.WriteLine($"🔍 Id a actualizar: {response.Id}");
        await _facturaService.ActualizarNotaCreditoDatosAfipAsync(response.Id, respuestaAfip);

        // 5️⃣ Commit
        await efTransaction.CommitAsync();

        return Ok(new
        {
            response.Id,
            cae = respuestaAfip.Cae,
            vencimiento = respuestaAfip.CaeVencimiento,
            observaciones = respuestaAfip.Observaciones
        });
    }
    catch (Exception ex)
    {
        await efTransaction.RollbackAsync();
        return BadRequest(new { error = ex.Message });
    }
}
}

