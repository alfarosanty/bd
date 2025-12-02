using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

public class ComprobanteCaeBuilder
{
    private int tipoComprobante { get; set;}
    private int puntoVenta { get; set;} 
    private int numeroComprobante { get; set;} 
    private string fechaEmision { get; set;} 

    private int tipoDoc { get; set;} 
    private long nroDoc { get; set;} 
    private int condicionIVA { get; set;} 

    private decimal importeGravado { get; set;} 
    private decimal importeSubtotal { get; set;} 
    private decimal importeTotal { get; set;} 

    private List<Item> items { get; set;} = new List<Item>(); 
    private List<SubtotalIVA> subtotales { get; set;}  = new List<SubtotalIVA>();


    public List<Item> GetItems(){
        return items;
    }
    public ComprobanteCaeBuilder datosFactura(int tipoComprobante, int puntoVenta, int numero, DateTime fechaFactura)
    {
        this.tipoComprobante = tipoComprobante;
        this.puntoVenta = puntoVenta;
        this.numeroComprobante = numero;
        this.fechaEmision = fechaFactura.ToString("yyyy-MM-dd");
        return this;
    }

    public ComprobanteCaeBuilder Receptor(int tipoDoc, long nroDoc, int condicionIVA)
    {
        this.tipoDoc = tipoDoc;
        this.nroDoc = nroDoc;
        this.condicionIVA = condicionIVA;
        return this;
    }

    public ComprobanteCaeBuilder Importes(decimal gravado, decimal subtotal, decimal total)
    {
        this.importeGravado = gravado;
        this.importeSubtotal = subtotal;
        this.importeTotal = total;
        return this;
    }

    public ComprobanteCaeBuilder AgregarItem(Item item)
    {
        items.Add(item);
        return this;
    }

    public ComprobanteCaeBuilder AgregarSubtotal(SubtotalIVA sub)
    {
        subtotales.Add(sub);
        return this;
    }

    public string Build()
    {
        var sb = new StringBuilder();

        sb.AppendLine(@"<comprobanteCAERequest>");

        sb.AppendLine($"<codigoTipoComprobante>{tipoComprobante}</codigoTipoComprobante>");
        sb.AppendLine($"<numeroPuntoVenta>{puntoVenta}</numeroPuntoVenta>");
        sb.AppendLine($"<numeroComprobante>{numeroComprobante}</numeroComprobante>");
        sb.AppendLine($"<fechaEmision>{fechaEmision}</fechaEmision>");


        sb.AppendLine($"<codigoTipoDocumento>{tipoDoc}</codigoTipoDocumento>");
        sb.AppendLine($"<numeroDocumento>{nroDoc}</numeroDocumento>");
        sb.AppendLine($"<condicionIVAReceptor>{condicionIVA}</condicionIVAReceptor>");

        sb.AppendLine($"<importeGravado>{importeGravado}</importeGravado>");
        sb.AppendLine($"<importeSubtotal>{importeSubtotal}</importeSubtotal>");
        sb.AppendLine($"<importeTotal>{importeTotal}</importeTotal>");

        sb.AppendLine("<codigoMoneda>PES</codigoMoneda>");
        sb.AppendLine("<cotizacionMoneda>1</cotizacionMoneda>");
        sb.AppendLine("<codigoConcepto>1</codigoConcepto>");

        sb.AppendLine("<arrayItems>");

        foreach (var i in items)
        {
            sb.AppendLine("<item>");
            sb.AppendLine($"<unidadesMtx>{i.unidadesMtx}</unidadesMtx>");
            sb.AppendLine($"<codigoMtx>{i.codigoMtx}</codigoMtx>");
            sb.AppendLine($"<codigo>{i.codigo}</codigo>");
            sb.AppendLine($"<descripcion>{i.descripcion}</descripcion>");
            sb.AppendLine($"<cantidad>{i.cantidad}</cantidad>");
            sb.AppendLine($"<codigoUnidadMedida>{i.codigoUnidadMedida}</codigoUnidadMedida>");
            sb.AppendLine($"<precioUnitario>{i.precioUnitario}</precioUnitario>");
            sb.AppendLine($"<importeBonificacion>{i.importeBonificacion}</importeBonificacion>");
            sb.AppendLine($"<codigoCondicionIVA>{i.codigoCondicionIVA}</codigoCondicionIVA>");
            sb.AppendLine($"<importeIVA>{i.importeIVA}</importeIVA>");
            sb.AppendLine($"<importeItem>{i.importeItem}</importeItem>");
            sb.AppendLine("</item>");
        }

        sb.AppendLine("  </arrayItems>");

        sb.AppendLine("  <arraySubtotalesIVA>");
        foreach (var s in subtotales)
        {
            sb.AppendLine("<subtotalIVA>");
            sb.AppendLine($"<codigo>{s.codigo}</codigo>");
            sb.AppendLine($"<importe>{s.importe}</importe>");
            sb.AppendLine("</subtotalIVA>");
        }
        sb.AppendLine("  </arraySubtotalesIVA>");

        sb.AppendLine("</comprobanteCAERequest>");

        return sb.ToString();
    }
}

public class Item
{
    public long unidadesMtx { get; set; }
    public string codigoMtx { get; set; }
    public string codigo { get; set; }
    public string descripcion { get; set; }
    public decimal cantidad { get; set; }
    public int codigoUnidadMedida { get; set; }
    public decimal precioUnitario { get; set; }
    public decimal importeBonificacion { get; set; }
    public int codigoCondicionIVA { get; set; }
    public decimal importeIVA { get; set; }
    public decimal importeItem { get; set; }
}

public class ArraySubtotalesIVA
{
    [XmlElement("subtotalIVA")]
    public List<SubtotalIVA> Subtotales { get; set; }
}

public class SubtotalIVA
{
    public int codigo { get; set; }
    public decimal importe { get; set; }
}


