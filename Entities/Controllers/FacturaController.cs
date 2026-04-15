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

    [HttpPost()]
    public async Task<ActionResult>  Crear(Factura factura){

        int id =  await _facturaService.Crear(factura, false);
        factura.Id = id;
        return Ok(new
        {
            idInterno = id,
            respuestaCompleta = factura
        });    
    }

[HttpPost("ARCA")]
    public async Task<ActionResult> CrearConARCAAsync([FromBody] Factura factura)
    {
        if (factura == null) return BadRequest("Factura inválida.");

        try
        {

            int idInterno = await _facturaService.Crear(factura, true);
            
            return Ok(new { idInterno, mensaje = "Factura autorizada correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

[HttpGet("{id}/pdf")]
public async Task<IActionResult> PdfFacturaAsync(int id)
{
    var srv = new FastReportService(_afipClient, _facturaService);

    Factura factura = await _facturaService.GetByIdAsync(id);

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

