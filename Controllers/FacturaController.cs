using Microsoft.AspNetCore.Mvc;
using Npgsql;
using BlumeAPI.Services;  // Cambia esto si tu servicio está en otro namespace
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

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
        [HttpGet("autenticar")]
        public IActionResult Autenticar()
        {
            try
            {
                string servicio = "wsfe";    // servicio al que queremos acceder
                string pfxPath = @"C:\certificados\afip_homo.pfx"; // ruta al certificado de homologación
                string pfxPassword = "tu_clave"; // contraseña del certificado

                // 1) Generar TRA
                string traXml = GenerarTRA(servicio);

                // 2) Firmar el TRA
                string traFirmado = FirmarTRA(traXml, pfxPath, pfxPassword);

                // 3) Llamar al WSAA en homologación
                var client = new AfipWsaa.LoginCMSService();
                client.Url = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms";

                string loginCmsResponse = client.loginCms(traFirmado);

                // 4) Parsear respuesta
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(loginCmsResponse);

                string token = xml.SelectSingleNode("//token")?.InnerText;
                string sign = xml.SelectSingleNode("//sign")?.InnerText;
                string expiration = xml.SelectSingleNode("//expirationTime")?.InnerText;

                return Ok(new
                {
                    Token = token,
                    Sign = sign,
                    Expira = expiration
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error autenticando en AFIP: " + ex.Message);
            }
        }

        private string GenerarTRA(string servicio)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml($@"
                <loginTicketRequest version=""1.0"">
                  <header>
                    <uniqueId>{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}</uniqueId>
                    <generationTime>{DateTime.UtcNow.AddMinutes(-10):s}</generationTime>
                    <expirationTime>{DateTime.UtcNow.AddMinutes(10):s}</expirationTime>
                  </header>
                  <service>{servicio}</service>
                </loginTicketRequest>");
            return xml.OuterXml;
        }

        private string FirmarTRA(string traXml, string pfxPath, string pfxPassword)
        {
            X509Certificate2 cert = new X509Certificate2(pfxPath, pfxPassword,
                X509KeyStorageFlags.Exportable);

            ContentInfo info = new ContentInfo(Encoding.UTF8.GetBytes(traXml));
            SignedCms cms = new SignedCms(info);

            CmsSigner signer = new CmsSigner(cert);
            cms.ComputeSignature(signer);
            byte[] firma = cms.Encode();

            return Convert.ToBase64String(firma);
        }
*/
}

