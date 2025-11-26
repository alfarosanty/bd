using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class AfipWsMtxcaClient
{
    private readonly HttpClient _http;
    private readonly string _endpoint;

    public AfipWsMtxcaClient(string endpoint)
    {
        _endpoint = endpoint;
        _http = new HttpClient();
    }


    // ============================================================
    // MÉTODO PRIVADO: Enviar XML SOAP
    // ============================================================
    private async Task<string> SendSoapRequest(string soapAction, string xmlBody)
    {
        var envelope = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                  xmlns:ser=""http://impl.service.wsmtxca.afip.gov.ar/service/"">
   <soapenv:Header/>
   <soapenv:Body>
      {xmlBody}
   </soapenv:Body>
</soapenv:Envelope>";

        Console.WriteLine("===== XML ENVIADO =====");
        Console.WriteLine(envelope);

        var content = new StringContent(envelope, Encoding.UTF8, "text/xml");
        //content.Headers.Add("SOAPAction", soapAction);

        var response = await _http.PostAsync(_endpoint, content);
        string text = await response.Content.ReadAsStringAsync();

        Console.WriteLine("===== XML RECIBIDO =====");
        Console.WriteLine(text);

        response.EnsureSuccessStatusCode();

        return text;
    }


    // ============================================================
    // MÉTODO 1: Dummy
    // ============================================================
    public async Task<string> DummyAsync()
    {
        string body = @"<ser:dummy/>";

        return await SendSoapRequest(
            "http://impl.service.wsmtxca.afip.gov.ar/service/dummy",
            body
        );
    }


    // ============================================================
    // MÉTODO 2: Autorizar Comprobante
    // ============================================================
    public async Task<string> AutorizarComprobanteAsync(
        string token, 
        string sign, 
        long cuit, 
        string comprobanteCaeXml // XML tal cual lo armas vos
    )
    {
        // authRequest SIN namespaces, tal cual tu XML
        string body = $@"
<ser:autorizarComprobanteRequest>

   <authRequest>
      <token>{token}</token>
      <sign>{sign}</sign>
      <cuitRepresentada>{cuit}</cuitRepresentada>
   </authRequest>

   {comprobanteCaeXml}

</ser:autorizarComprobanteRequest>";

        return await SendSoapRequest(
            "http://impl.service.wsmtxca.afip.gov.ar/service/autorizarComprobante",
            body
        );
    }



    // ============================================================
    // MÉTODO 3: Consultar último comprobante autorizado
    // ============================================================
    public async Task<string> ConsultarUltimoAutorizadoAsync(long cuit, int tipoCmp, int puntoVta)
    {
        string body = $@"
<ser:consultarUltimoComprobanteAutorizadoRequest>
   <authRequest>
      <token>NO_APLICA</token>
      <sign>NO_APLICA</sign>
      <cuitRepresentada>{cuit}</cuitRepresentada>
   </authRequest>

   <codigoTipoComprobante>{tipoCmp}</codigoTipoComprobante>
   <numeroPuntoVenta>{puntoVta}</numeroPuntoVenta>
</ser:consultarUltimoComprobanteAutorizadoRequest>";

        return await SendSoapRequest(
            "http://impl.service.wsmtxca.afip.gov.ar/service/consultarUltimoComprobanteAutorizado",
            body
        );
    }
}
