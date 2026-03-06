using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

public class ComprobanteCaeBuilderWsfe
{
    private int tipoComprobante;
    private int puntoVenta;
    private int numeroComprobante;
    private string fechaEmision;

    private int tipoDoc;
    private long nroDoc;
    private int tipoCondIVAReceptor;

    private decimal importeGravado;
    private decimal importeTotal;

    private int concepto = 1; // 1 = Productos

    private List<SubtotalIVA> subtotales = new();

    // ============================
    // DATOS FACTURA
    // ============================
    public ComprobanteCaeBuilderWsfe DatosFactura(
        int tipoComprobante,
        int puntoVenta,
        int numero,
        DateTime fechaFactura)
    {
        this.tipoComprobante = tipoComprobante;
        this.puntoVenta = puntoVenta;
        this.numeroComprobante = numero;
        this.fechaEmision = fechaFactura.ToString("yyyyMMdd"); // WSFE formato requerido
        return this;
    }

    // ============================
    // RECEPTOR
    // ============================
    public ComprobanteCaeBuilderWsfe Receptor(int tipoDoc, long nroDoc, int tipoCondIVAReceptor)
    {
        this.tipoDoc = tipoDoc;
        this.nroDoc = nroDoc;
        this.tipoCondIVAReceptor = tipoCondIVAReceptor;
        return this;
    }

    // ============================
    // IMPORTES
    // ============================
    public ComprobanteCaeBuilderWsfe Importes(decimal gravado, decimal total)
    {
        this.importeGravado = gravado;
        this.importeTotal = total;
        return this;
    }

    // ============================
    // IVA
    // ============================
    public ComprobanteCaeBuilderWsfe AgregarSubtotalIVA(SubtotalIVA sub)
    {
        subtotales.Add(sub);
        return this;
    }

    // ============================
    // BUILD XML WSFE
    // ============================
    public string Build()
    {
        var sb = new StringBuilder();

        decimal ivaTotal = subtotales.Sum(s => s.importe);

        sb.AppendLine("<ser:FeCAEReq>");

        // CABECERA
        sb.AppendLine("<ser:FeCabReq>");
        sb.AppendLine("<ser:CantReg>1</ser:CantReg>");
        sb.AppendLine($"<ser:PtoVta>{puntoVenta}</ser:PtoVta>");
        sb.AppendLine($"<ser:CbteTipo>{tipoComprobante}</ser:CbteTipo>");
        sb.AppendLine("</ser:FeCabReq>");

        // DETALLE
        sb.AppendLine("<ser:FeDetReq>");
        sb.AppendLine("<ser:FECAEDetRequest>");

        sb.AppendLine($"<ser:Concepto>{concepto}</ser:Concepto>");
        sb.AppendLine($"<ser:DocTipo>{tipoDoc}</ser:DocTipo>");
        sb.AppendLine($"<ser:DocNro>{nroDoc}</ser:DocNro>");
        sb.AppendLine($"<ser:CondicionIVAReceptorId>{tipoCondIVAReceptor}</ser:CondicionIVAReceptorId>");
        sb.AppendLine($"<ser:CbteDesde>{numeroComprobante}</ser:CbteDesde>");
        sb.AppendLine($"<ser:CbteHasta>{numeroComprobante}</ser:CbteHasta>");
        sb.AppendLine($"<ser:CbteFch>{fechaEmision}</ser:CbteFch>");

        sb.AppendLine($"<ser:ImpTotal>{numberToString(importeTotal)}</ser:ImpTotal>");
        sb.AppendLine("<ser:ImpTotConc>0</ser:ImpTotConc>");
        sb.AppendLine($"<ser:ImpNeto>{numberToString(importeGravado)}</ser:ImpNeto>");
        sb.AppendLine("<ser:ImpOpEx>0</ser:ImpOpEx>");
        sb.AppendLine($"<ser:ImpIVA>{numberToString(ivaTotal)}</ser:ImpIVA>");
        sb.AppendLine("<ser:ImpTrib>0</ser:ImpTrib>");

        sb.AppendLine("<ser:MonId>PES</ser:MonId>");
        sb.AppendLine("<ser:MonCotiz>1</ser:MonCotiz>");

        // IVA detalle
        if (subtotales.Any())
        {
            sb.AppendLine("<ser:Iva>");

            foreach (var s in subtotales)
            {
                sb.AppendLine("<ser:AlicIva>");
                sb.AppendLine($"<ser:Id>{s.codigo}</ser:Id>");
                sb.AppendLine($"<ser:BaseImp>{numberToString(importeGravado)}</ser:BaseImp>");
                sb.AppendLine($"<ser:Importe>{numberToString(s.importe)}</ser:Importe>");
                sb.AppendLine("</ser:AlicIva>");
            }

            sb.AppendLine("</ser:Iva>");
        }

        sb.AppendLine("</ser:FECAEDetRequest>");
        sb.AppendLine("</ser:FeDetReq>");

        sb.AppendLine("</ser:FeCAEReq>");

        return sb.ToString();
    }

    private string numberToString(decimal valor)
    {
        return valor.ToString("0.00", CultureInfo.InvariantCulture);
    }
}

// ============================
// Subtotal IVA (igual que antes)
// ============================
public class SubtotalIVA
{
    public int codigo { get; set; }   // 5 = 21%, 4 = 10.5%, etc
    public decimal importe { get; set; }
}