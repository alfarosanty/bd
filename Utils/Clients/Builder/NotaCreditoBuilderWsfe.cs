using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public class NotaCreditoBuilderWsfe
{
    // ============================
    // TIPOS SOPORTADOS
    // ============================
    public static class Tipos
    {
        public const int NotaCreditoA = 3;
        public const int NotaCreditoB = 8;
    }

    // ============================
    // CAMPOS INTERNOS
    // ============================
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

    // Comprobante asociado (obligatorio desde 01/04/2021)
    private int asocTipo;
    private int asocPtoVta;
    private long asocNro;
    private long asocCuit;
    private string asocFecha;

    // ============================
    // DATOS NOTA DE CRÉDITO
    // ============================
    public NotaCreditoBuilderWsfe DatosNotaCredito(
        int tipoComprobante,
        int puntoVenta,
        int numero,
        DateTime fechaEmision)
    {
        if (tipoComprobante != Tipos.NotaCreditoA && tipoComprobante != Tipos.NotaCreditoB)
            throw new ArgumentException($"Tipo de comprobante inválido: {tipoComprobante}. Usar NotaCreditoA (3) o NotaCreditoB (8).");

        this.tipoComprobante = tipoComprobante;
        this.puntoVenta = puntoVenta;
        this.numeroComprobante = numero;
        this.fechaEmision = fechaEmision.ToString("yyyyMMdd");
        return this;
    }

    // ============================
    // RECEPTOR
    // ============================
    public NotaCreditoBuilderWsfe Receptor(int tipoDoc, long nroDoc, int tipoCondIVAReceptor)
    {
        this.tipoDoc = tipoDoc;
        this.nroDoc = nroDoc;
        this.tipoCondIVAReceptor = tipoCondIVAReceptor;
        return this;
    }

    // ============================
    // IMPORTES
    // ============================
    public NotaCreditoBuilderWsfe Importes(decimal gravado, decimal total)
    {
        this.importeGravado = gravado;
        this.importeTotal = total;
        return this;
    }

    // ============================
    // IVA
    // ============================
    public NotaCreditoBuilderWsfe AgregarSubtotalIVA(SubtotalIVA sub)
    {
        subtotales.Add(sub);
        return this;
    }

    // ============================
    // COMPROBANTE ASOCIADO (obligatorio)
    // ============================
    public NotaCreditoBuilderWsfe ComprobanteAsociado(
        int tipo,
        int ptoVta,
        long nro,
        long cuit,
        DateTime fecha)
    {
        this.asocTipo = tipo;
        this.asocPtoVta = ptoVta;
        this.asocNro = nro;
        this.asocCuit = cuit;
        this.asocFecha = fecha.ToString("yyyyMMdd");
        return this;
    }

    // ============================
    // BUILD XML WSFE
    // ============================
    public string Build()
    {
        ValidarCamposObligatorios();

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

        sb.AppendLine($"<ser:ImpTotal>{ToStr(importeTotal)}</ser:ImpTotal>");
        sb.AppendLine("<ser:ImpTotConc>0</ser:ImpTotConc>");
        sb.AppendLine($"<ser:ImpNeto>{ToStr(importeGravado)}</ser:ImpNeto>");
        sb.AppendLine("<ser:ImpOpEx>0</ser:ImpOpEx>");
        sb.AppendLine($"<ser:ImpIVA>{ToStr(ivaTotal)}</ser:ImpIVA>");
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
                sb.AppendLine($"<ser:BaseImp>{ToStr(importeGravado)}</ser:BaseImp>");
                sb.AppendLine($"<ser:Importe>{ToStr(s.importe)}</ser:Importe>");
                sb.AppendLine("</ser:AlicIva>");
            }
            sb.AppendLine("</ser:Iva>");
        }

        // COMPROBANTE ASOCIADO (obligatorio para NC)
        sb.AppendLine("<ser:CbtesAsoc>");
        sb.AppendLine("<ser:CbteAsoc>");
        sb.AppendLine($"<ser:Tipo>{asocTipo}</ser:Tipo>");
        sb.AppendLine($"<ser:PtoVta>{asocPtoVta}</ser:PtoVta>");
        sb.AppendLine($"<ser:Nro>{asocNro}</ser:Nro>");
        sb.AppendLine($"<ser:Cuit>{asocCuit}</ser:Cuit>");
        sb.AppendLine($"<ser:CbteFch>{asocFecha}</ser:CbteFch>");
        sb.AppendLine("</ser:CbteAsoc>");
        sb.AppendLine("</ser:CbtesAsoc>");

        sb.AppendLine("</ser:FECAEDetRequest>");
        sb.AppendLine("</ser:FeDetReq>");
        sb.AppendLine("</ser:FeCAEReq>");

        return sb.ToString();
    }

    // ============================
    // VALIDACIONES
    // ============================
    private void ValidarCamposObligatorios()
    {
        if (tipoComprobante == 0)
            throw new InvalidOperationException("Falta llamar a DatosNotaCredito().");

        if (nroDoc == 0)
            throw new InvalidOperationException("Falta llamar a Receptor().");

        if (importeTotal == 0)
            throw new InvalidOperationException("Falta llamar a Importes().");

        if (asocNro == 0)
            throw new InvalidOperationException("Falta llamar a ComprobanteAsociado(). Es obligatorio para notas de crédito.");
    }

    private string ToStr(decimal valor) =>
        valor.ToString("0.00", CultureInfo.InvariantCulture);
}