using System.Text;
using System.Text.Json;

public static class AfipQrHelper
{
    public static string GenerarQrUrl(Factura factura, long cuitEmisor)
    {
        if (factura == null)
            throw new ArgumentNullException(nameof(factura));

        if (!factura.CaeNumero.HasValue)
            throw new InvalidOperationException("La factura no posee CAE");

        int tipoComprobante = factura.TipoFactura switch
        {
            "A" => 1,
            "B" => 6,
            "C" => 11,
            _ => throw new Exception($"Tipo de factura inválido: {factura.TipoFactura}")
        };


        var qrData = new
        {
            ver = 1,
            fecha = factura.FechaFactura.ToString("yyyy-MM-dd"),
            cuit = cuitEmisor,
            ptoVta = factura.PuntoDeVenta,
            tipoCmp = tipoComprobante,
            nroCmp = factura.NumeroComprobante,
            importe = Math.Round((decimal)factura.ImporteBruto, 2),
            moneda = "PES",
            ctz = 1,
            tipoDocRec = 80,
            nroDocRec = factura.Cliente.Cuit,
            tipoCodAut = "E",
            codAut = factura.CaeNumero
        };

        string json = JsonSerializer.Serialize(qrData);
        string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        return $"https://www.afip.gob.ar/fe/qr/?p={base64}";
    }
}
