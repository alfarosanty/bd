using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public class AfipWsfeClient
{
    private readonly HttpClient _http;
    private readonly string _endpoint;

    public AfipWsfeClient(string endpoint)
    {
        _endpoint = endpoint;
        _http = new HttpClient();
    }

    // ============================================================
    // MÉTODO PRIVADO: Enviar XML SOAP
    // ============================================================
private async Task<string> SendSoapRequest(string xmlBody)
{
    var envelope = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                  xmlns:ser=""http://ar.gov.afip.dif.FEV1/"">
   <soapenv:Header/>
   <soapenv:Body>
      {xmlBody}
   </soapenv:Body>
</soapenv:Envelope>";

    Console.WriteLine("===== XML ENVIADO =====");
    Console.WriteLine(envelope);

    var content = new StringContent(envelope, Encoding.UTF8, "text/xml");

    var response = await _http.PostAsync(_endpoint, content);

    string text = await response.Content.ReadAsStringAsync();

    Console.WriteLine("===== STATUS CODE =====");
    Console.WriteLine(response.StatusCode);

    Console.WriteLine("===== XML RECIBIDO =====");
    Console.WriteLine(text);

    return text;
}

    // ============================================================
    // MÉTODO 1: Dummy
    // ============================================================
    public async Task<string> DummyAsync()
    {
        string body = @"<ser:FEDummy/>";
        return await SendSoapRequest(body);
    }

    // ============================================================
    // MÉTODO 2: Autorizar comprobante
    // ============================================================
public async Task<AfipResponse> AutorizarComprobanteAsync(
    string token,
    string sign,
    long cuit,
    string feCaeReqXml,
    int idFactura)
{
    string body = $@"
<ser:FECAESolicitar>
   <ser:Auth>
      <ser:Token>{token}</ser:Token>
      <ser:Sign>{sign}</ser:Sign>
      <ser:Cuit>{cuit}</ser:Cuit>
   </ser:Auth>
   {feCaeReqXml}
</ser:FECAESolicitar>";

    var xml = await SendSoapRequest(body);

    var response = ParseAutorizarResponse(xml, idFactura);

    if (!response.Aprobado)
    {
        Console.WriteLine("=== ERRORES AFIP ===");

        foreach (var err in response.Errores)
            Console.WriteLine(err);

        foreach (var obs in response.Observaciones)
            Console.WriteLine("OBS: " + obs);
    }

    return response;
}

    // ============================================================
    // MÉTODO 3: Último comprobante autorizado
    // ============================================================
public async Task<UltimoComprobanteAutorizadoResult> ConsultarUltimoAutorizadoAsync(
    string token,
    string sign,
    long cuit,
    int puntoVta,
    int tipoCbte)
{
    string body = $@"
<ser:FECompUltimoAutorizado>
   <ser:Auth>
      <ser:Token>{token}</ser:Token>
      <ser:Sign>{sign}</ser:Sign>
      <ser:Cuit>{cuit}</ser:Cuit>
   </ser:Auth>
   <ser:PtoVta>{puntoVta}</ser:PtoVta>
   <ser:CbteTipo>{tipoCbte}</ser:CbteTipo>
</ser:FECompUltimoAutorizado>";

    var xml = await SendSoapRequest(body);

    return ParseUltimoResponse(xml);
}

    // ============================================================
    // PARSE RESPONSE FECAESolicitar
    // ============================================================
public AfipResponse ParseAutorizarResponse(string xml, int idFactura)
{
    var doc = XDocument.Parse(xml);
    var result = new AfipResponse
    {
        idFactura = idFactura
    };

    var resultado = doc.Descendants()
        .FirstOrDefault(x => x.Name.LocalName == "Resultado")?.Value;

    result.Aprobado = resultado == "A";

    result.Cae = doc.Descendants()
        .FirstOrDefault(x => x.Name.LocalName == "CAE")?.Value;

    result.CaeVencimiento = doc.Descendants()
        .FirstOrDefault(x => x.Name.LocalName == "CAEFchVto")?.Value;

    result.numeroComprobante = doc.Descendants()
        .FirstOrDefault(x => x.Name.LocalName == "CbteDesde")?.Value;

    var errores = doc.Descendants()
        .Where(x => x.Name.LocalName == "Err");

    foreach (var err in errores)
    {
        var code = err.Elements()
            .FirstOrDefault(x => x.Name.LocalName == "Code")?.Value;

        var msg = err.Elements()
            .FirstOrDefault(x => x.Name.LocalName == "Msg")?.Value;

        result.Errores.Add($"{code} - {msg}");
    }

    var observaciones = doc.Descendants()
        .Where(x => x.Name.LocalName == "Obs");

    foreach (var obs in observaciones)
    {
        var code = obs.Elements()
            .FirstOrDefault(x => x.Name.LocalName == "Code")?.Value;

        var msg = obs.Elements()
            .FirstOrDefault(x => x.Name.LocalName == "Msg")?.Value;

        result.Observaciones.Add($"{code} - {msg}");
    }

    return result;
}

    // ============================================================
    // PARSE RESPONSE FECompUltimoAutorizado
    // ============================================================
    public UltimoComprobanteAutorizadoResult ParseUltimoResponse(string xml)
{
    var result = new UltimoComprobanteAutorizadoResult();
    var doc = XDocument.Parse(xml);

    // =========================
    // BUSCAR CbteNro
    // =========================
    var nro = doc.Descendants()
        .FirstOrDefault(x => x.Name.LocalName == "CbteNro")?.Value;

    if (!string.IsNullOrWhiteSpace(nro) && int.TryParse(nro, out int numero))
    {
        result.Exitoso = true;
        result.NumeroComprobante = numero;
        return result;
    }

    // =========================
    // ERRORES
    // =========================
    var errores = doc.Descendants()
        .Where(x => x.Name.LocalName == "Err");

    foreach (var err in errores)
    {
        var codeStr = err.Elements()
            .FirstOrDefault(x => x.Name.LocalName == "Code")?.Value;

        var msg = err.Elements()
            .FirstOrDefault(x => x.Name.LocalName == "Msg")?.Value;

        int.TryParse(codeStr, out int code);

        result.Errores.Add(new AfipError
        {
            Codigo = code,
            Descripcion = msg
        });
    }

    result.Exitoso = false;
    result.NumeroComprobante = null;

    return result;
}
}