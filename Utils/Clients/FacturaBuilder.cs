using BlumeAPI.Services;
using BlumeAPI.servicios;

public class FacturaBuilder
{
    private readonly ITemplateService _templateService;
    private readonly IPdfService _pdfService;

    FacturaServices facturaServices = new FacturaServices();

    public FacturaBuilder(
        ITemplateService templateService,
        IPdfService pdfService)
    {
        _templateService = templateService;
        _pdfService = pdfService;
    }

public async Task<byte[]> Build(Factura factura)
{
    var html = _templateService.loadTemplate("Factura.html");

    var data = new Dictionary<string, string>
    {
        { "puntoDeVenta", factura.PuntoDeVenta.ToString() },
        { "numeroComprobante", factura.NumeroComprobante.ToString() },
        { "fechaFactura", factura.FechaFactura.ToString() },
        { "numeroCAE", factura.CaeNumero.ToString() },
        { "fechaVencimientoCAE", factura.FechaVencimientoCae.ToString() },
        { "cuitCliente", factura.Cliente.Cuit },
        { "clienteRazonSocial", factura.Cliente.RazonSocial },
        { "clienteCondicionIVA", factura.Cliente.CondicionFiscal.Descripcion },
        { "clienteDomicilio", factura.Cliente.Domicilio },
        { "subtotal", factura.ImporteNeto.GetValueOrDefault().ToString("N2") },
        { "iva", (factura.ImporteBruto - factura.ImporteNeto).GetValueOrDefault().ToString("N2") },
        { "total", factura.ImporteBruto.GetValueOrDefault().ToString("N2") }
    };

    html = _templateService.applyData(html, data);

    var articulosAgrupados = facturaServices.AgruparPorCodigo(factura.Articulos);
    var detalleHtml = _templateService.renderDetalleArticulos(articulosAgrupados);

    html = html.Replace("{{DETALLE_ARTICULOS}}", detalleHtml);

    return await _pdfService.convertHtmlToPdfAsync(html);
}


}
