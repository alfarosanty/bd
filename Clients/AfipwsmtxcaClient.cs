using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
    public async Task<string> ConsultarUltimoAutorizadoAsync(string token, string sign, long cuit, int tipoCmp, int puntoVta)
    {
        string body = $@"
<ser:consultarUltimoComprobanteAutorizadoRequest>
   <authRequest>
      <token>{token}</token>
      <sign>{sign}</sign>
      <cuitRepresentada>{cuit}</cuitRepresentada>
   </authRequest>

 <consultaUltimoComprobanteAutorizadoRequest>
   <codigoTipoComprobante>{tipoCmp}</codigoTipoComprobante>
   <numeroPuntoVenta>{puntoVta}</numeroPuntoVenta>
 </consultaUltimoComprobanteAutorizadoRequest>
</ser:consultarUltimoComprobanteAutorizadoRequest>";

        return await SendSoapRequest(
            "http://impl.service.wsmtxca.afip.gov.ar/service/consultarUltimoComprobanteAutorizado",
            body
        );
    }

    // ============================================================
    // MÉTODO 4: Manejo de response autorizar factura
    // ============================================================

    public AfipResponse ParseAfipResponse(string xml)
{
    var doc = XDocument.Parse(xml);
    var result = new AfipResponse();

    // resultado A/R
    string resultado = doc.Descendants()
        .FirstOrDefault(x => x.Name.LocalName == "resultado")?.Value;

    result.Aprobado = resultado == "A" || resultado == "O";

    // errores
    var errores = doc.Descendants()
        .Where(x => x.Name.LocalName == "arrayErrores")
        .Descendants()
        .Where(x => x.Name.LocalName == "codigoDescripcion");

    foreach (var err in errores)
    {
        string codigo = err.Element(err.Name.Namespace + "codigo")?.Value;
        string desc = err.Element(err.Name.Namespace + "descripcion")?.Value;
        result.Errores.Add($"{codigo} - {desc}");
    }

    // observaciones (si las hay)
    var observaciones = doc.Descendants()
        .Where(x => x.Name.LocalName == "arrayObservaciones")
        .Descendants()
        .Where(x => x.Name.LocalName == "codigoDescripcion");

    foreach (var obs in observaciones)
    {
        string codigo = obs.Element(obs.Name.Namespace + "codigo")?.Value;
        string desc = obs.Element(obs.Name.Namespace + "descripcion")?.Value;
        result.Observaciones.Add($"{codigo} - {desc}");
    }

    // CAE
    result.Cae = doc.Descendants()
        .FirstOrDefault(x => x.Name.LocalName == "cae")?.Value;

    // vencimiento CAE
    result.CaeVencimiento = doc.Descendants()
        .FirstOrDefault(x => x.Name.LocalName == "fechaVencimientoCae")?.Value;

    return result;
}

    // ============================================================
    // MÉTODO 5: Manejo de response consultar último comprobante
    // ============================================================

public UltimoComprobanteAutorizadoResult ParseUltimoComprobanteAutorizadoResponse(string xml)
{
    var result = new UltimoComprobanteAutorizadoResult();

    var doc = XDocument.Parse(xml);
    XNamespace ns = "http://impl.service.wsmtxca.afip.gov.ar/service/";

    var resp = doc.Descendants(ns + "consultarUltimoComprobanteAutorizadoResponse").FirstOrDefault();
    if (resp == null)
        return new UltimoComprobanteAutorizadoResult
        {
            Exitoso = false,
            Errores = { new AfipError { Codigo = -1, Descripcion = "Respuesta inesperada del servidor" } }
        };

    // === Caso 1: hay errores ===
    var arrayErrores = resp.Element("arrayErrores");
    if (arrayErrores != null)
    {
        foreach (var item in arrayErrores.Elements("codigoDescripcion"))
        {
            result.Errores.Add(new AfipError
            {
                Codigo = int.Parse(item.Element("codigo")?.Value ?? "0"),
                Descripcion = item.Element("descripcion")?.Value
            });
        }

        result.Exitoso = false;
        return result;
    }

    // === Caso 2: respuesta correcta ===
    var numCmp = resp.Element("numeroComprobante")?.Value;
    if (!string.IsNullOrEmpty(numCmp))
    {
        result.Exitoso = true;
        result.NumeroComprobante = int.Parse(numCmp);
        return result;
    }

    // === Caso raro: sin error y sin número ===
    return new UltimoComprobanteAutorizadoResult
    {
        Exitoso = false,
        Errores = { new AfipError { Codigo = -2, Descripcion = "No se encontró ni número ni errores." } }
    };
}


}
