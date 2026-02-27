using BlumeAPI.Services;
using FastReport;
using FastReport.Export.PdfSimple;
using System.Globalization;
using System.IO;

public class FastReportService
{
    private static readonly CultureInfo CulturaAR = new("es-AR");

    public byte[] CrearPdf(Factura factura, string version)
    {
        using var report = new Report();

        // 🔹 Cultura argentina para TODO el reporte
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-AR");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-AR");
        string plantilla = factura.TipoFactura switch
        {
            "A" => "FastReport/Factura_A.frx",
            "B" => "FastReport/Factura_B.frx",
            _ => throw new Exception("Tipo de factura no soportado")
        };

        report.Dictionary.Clear();
        report.Load(plantilla);

        CargarParametrosBase(report, factura, version);
        CargarTotales(report, factura);
        RegistrarArticulos(report, factura);
        ConfigurarFooter(report, factura);
        ConfigurarQr(report, factura);


        report.Prepare();

        using var export = new PDFSimpleExport();
        using var ms = new MemoryStream();
        report.Export(export, ms);

        return ms.ToArray();
    }

    // ===============================
    // 🔹 PARÁMETROS GENERALES
    // ===============================
    private void CargarParametrosBase(Report report, Factura factura, string version)
    {
        var ubicacion = CapitalizarPalabras(
            $"{factura.Cliente.Domicilio} - {factura.Cliente.Localidad}, {factura.Cliente.Provincia}");

        report.SetParameterValue("Version", version);
        report.SetParameterValue("TipoFactura", factura.TipoFactura);
        report.SetParameterValue("PtoVenta", factura.PuntoDeVenta.ToString("D4"));
        report.SetParameterValue("NroComprobante", factura.NumeroComprobante?.ToString("D8"));
        report.SetParameterValue("FechaFactura", factura.FechaFactura.ToString("dd/MM/yyyy"));

        report.SetParameterValue("CuitCliente", factura.Cliente.Cuit);
        report.SetParameterValue("RazonSocialCliente", factura.Cliente.RazonSocial);
        report.SetParameterValue("CondIVACliente", factura.Cliente.CondicionFiscal.Descripcion);
        report.SetParameterValue("CondVentaCliente", "Otra");
        report.SetParameterValue("DomicilioCliente", ubicacion);

        report.SetParameterValue("NroCAE", factura.CaeNumero);
        report.SetParameterValue("FechaVtoCAE", factura.FechaVencimientoCae?.ToString("dd/MM/yyyy"));
    }

    // ===============================
    // 🔹 TOTALES (DECIMALES, NO STRING)
    // ===============================
    private void CargarTotales(Report report, Factura factura)
    {
        report.SetParameterValue("Subtotal", factura.calcularSubtotal());
        report.SetParameterValue("Descuento", factura.CalcularDescuento());
        report.SetParameterValue("MontoIVA", factura.calcularIva());
        report.SetParameterValue("Total", factura.calcularTotal());
    }

    // ===============================
    // 🔹 ARTÍCULOS
    // ===============================
    private void RegistrarArticulos(Report report, Factura factura)
    {
        var facturaServices = new FacturaServices();
        var mapar = facturaServices.AgruparPorCodigo(factura.Articulos);
        var articulos = facturaServices.ConstruirResumen(mapar);

        report.RegisterData(articulos, "Articulos");
    }

    // ===============================
    // 🔹 FOOTER (sin comparar cultura)
    // ===============================
    private void ConfigurarFooter(Report report, Factura factura)
    {
        var footer = report.FindObject("PageFooter1") as FastReport.PageFooterBand;

        if (footer != null)
        {
            footer.AfterLayout += (sender, e) =>
            {
                var text = report.FindObject("Text66") as FastReport.TextObject;
                var linea1 = report.FindObject("Line19") as FastReport.LineObject;
                var linea2 = report.FindObject("Line20") as FastReport.LineObject;

                if (text == null) return;

                var subtotalEsperado = factura.CalcularSubtotalSinDescuentoGeneral();
                var subtotalFormateado = subtotalEsperado.ToString("N2", CulturaAR);
                var textoEsperado = $"Subtotal: ${subtotalFormateado}";

                /* 🔎 LOGS DE COMPARACIÓN
                Console.WriteLine("------ DEBUG FOOTER ------");
                Console.WriteLine($"Texto reporte: '{text.Text.Trim()}'");
                Console.WriteLine($"Texto esperado: '{textoEsperado}'");
                Console.WriteLine($"Coinciden: {text.Text.Trim() == textoEsperado}");
                Console.WriteLine("--------------------------");
                */
                if (text.Text.Trim() == textoEsperado)
                {
                    text.Visible = false;
                    linea1.Visible = false;
                    linea2.Visible = false;
                }
            };
        }
    }

    // ===============================
    // 🔹 QR
    // ===============================
    private void ConfigurarQr(Report report, Factura factura)
    {
        if (factura.CaeNumero.HasValue)
        {
            var qrUrl = AfipQrHelper.GenerarQrUrl(factura, 20302367613);
            report.SetParameterValue("QrText", qrUrl);
        }
        else
        {
            report.SetParameterValue("QrText", "");
        }
    }

    private string CapitalizarPalabras(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return texto;

        texto = texto.ToLower();
        return CulturaAR.TextInfo.ToTitleCase(texto);
    }
}