using BlumeAPI.Services;
using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using System.Drawing;
using System.Globalization;
using System.IO;

public class FastReportService
{

public byte[] CrearPdf(Factura factura)
{
    using var report = new Report();

    if (factura.TipoFactura == "A"){

    report.Dictionary.Clear();
    report.Load("FastReport/Factura_A.frx");

    var ubicacion = CapitalizarPalabras($"{factura.Cliente.Domicilio} - {factura.Cliente.Localidad}, {factura.Cliente.Provincia}");

        report.SetParameterValue("TipoFactura", factura.TipoFactura);
        report.SetParameterValue("PtoVenta", factura.PuntoDeVenta.ToString("D4"));
        report.SetParameterValue("NroComprobante", factura.NumeroComprobante?.ToString("D8"));
        report.SetParameterValue("FechaFactura", factura.FechaFactura.ToString("dd/MM/yyyy"));

        report.SetParameterValue("CuitCliente", factura.Cliente.Cuit);
        report.SetParameterValue("RazonSocialCliente", factura.Cliente.RazonSocial);
        report.SetParameterValue("CondIVACliente", factura.Cliente.CondicionFiscal.Descripcion);
        report.SetParameterValue("CondVentaCliente", "Otra");//factura.Cliente.CondicionFiscal.Descripcion);
        report.SetParameterValue("DomicilioCliente", ubicacion);

        report.SetParameterValue("NroCAE", factura.CaeNumero);
        report.SetParameterValue("FechaVtoCAE", factura.FechaVencimientoCae?.ToString("dd/MM/yyyy"));

        //TOTALES
        var total = factura.calcularTotal();
        var subtotal = factura.calcularSubtotal();
        var iva = factura.calcularIva();
        var descuento = factura.CalcularDescuento();
        report.SetParameterValue("Subtotal", subtotal.ToString("N2", new CultureInfo("es-AR")));
        report.SetParameterValue("Descuento", descuento.ToString("N2", new CultureInfo("es-AR")));
        report.SetParameterValue("MontoIVA", iva.ToString("N2", new CultureInfo("es-AR")));
        report.SetParameterValue("Total", total.ToString("N2", new CultureInfo("es-AR")));


// registrar datos
FacturaServices facturaServices = new FacturaServices();

var mapar = facturaServices.AgruparPorCodigo(factura.Articulos);
var articulos = facturaServices.ConstruirResumen(mapar);

report.RegisterData(articulos, "Articulos");
//var ds = report.GetDataSource("Articulos");
//ds.Enabled = true;

if (factura.CaeNumero.HasValue)
{
    var qrUrl = AfipQrHelper.GenerarQrUrl(factura, 20302367613);
    report.SetParameterValue("QrText", qrUrl);
}
else
{
    report.SetParameterValue("QrText", "");
}




report.Prepare();
using var export = new PDFSimpleExport();
using var ms = new MemoryStream();
report.Export(export, ms);

return ms.ToArray();
}   else
if(factura.TipoFactura == "B"){

    report.Dictionary.Clear();
    report.Load("FastReport/Factura_B.frx");

    // parámetros → OK

var ubicacion = CapitalizarPalabras($"{factura.Cliente.Domicilio} - {factura.Cliente.Localidad}, {factura.Cliente.Provincia}");
        report.SetParameterValue("TipoFactura", factura.TipoFactura);
        report.SetParameterValue("PtoVenta", factura.PuntoDeVenta.ToString("D4"));
        report.SetParameterValue("NroComprobante", factura.NumeroComprobante?.ToString("D8"));
        report.SetParameterValue("FechaFactura", factura.FechaFactura.ToString("dd/MM/yyyy"));

        report.SetParameterValue("CuitCliente", factura.Cliente.Cuit);
        report.SetParameterValue("RazonSocialCliente", factura.Cliente.RazonSocial);
        report.SetParameterValue("CondIVACliente", factura.Cliente.CondicionFiscal.Descripcion);
        report.SetParameterValue("CondVentaCliente", "Otra");//factura.Cliente.CondicionFiscal.Descripcion);
        report.SetParameterValue("DomicilioCliente", ubicacion);

        report.SetParameterValue("NroCAE", factura.CaeNumero);
        report.SetParameterValue("FechaVtoCAE", factura.FechaVencimientoCae?.ToString("dd/MM/yyyy"));

        //TOTALES
        var total = factura.calcularTotal();
        var subtotal = factura.calcularTotal();
        var descuento = factura.CalcularDescuento();
        report.SetParameterValue("Subtotal", subtotal.ToString("N2", new CultureInfo("es-AR")));
        report.SetParameterValue("Descuento", descuento.ToString("N2", new CultureInfo("es-AR")));
        report.SetParameterValue("Total", total.ToString("N2", new CultureInfo("es-AR")));

// registrar datos
// registrar datos
FacturaServices facturaServices = new FacturaServices();

var mapar = facturaServices.AgruparPorCodigo(factura.Articulos);
var articulos = facturaServices.ConstruirResumen(mapar);

report.RegisterData(articulos, "Articulos");

if (factura.CaeNumero.HasValue)
{
    var qrUrl = AfipQrHelper.GenerarQrUrl(factura, 20302367613);
    report.SetParameterValue("QrText", qrUrl);
}
else
{
    report.SetParameterValue("QrText", "");
}


report.Prepare();
using var export = new PDFSimpleExport();
using var ms = new MemoryStream();
report.Export(export, ms);

return ms.ToArray();
}
    throw new System.Exception("Tipo de factura no soportado (no es A ni B)");
}


string CapitalizarPalabras(string texto)
{
    if (string.IsNullOrWhiteSpace(texto))
        return texto;

    texto = texto.ToLower();
    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto);
}
}