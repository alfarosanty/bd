using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class Factura
    {
    public static String TABLA="FACTURA";
    public int Id { get; set; }

    public DateTime FechaFactura { get; set; }

    public Cliente Cliente { get; set; }

    public bool EximirIVA { get; set; }

    public List<ArticuloFactura>? Articulos  { get; set; }

    public Presupuesto? Presupuesto{ get; set; }

    public decimal? ImporteBruto  { get; set; }

    public int PuntoDeVenta { get; set; }

    public int? NumeroComprobante { get; set; }

    public long? CaeNumero { get; set; }

    public DateTime? FechaVencimientoCae { get; set; }

    public decimal? ImporteNeto  { get; set; }

    public int? Iva { get; set; }

    public string TipoFactura   { get; set; }

    public int? DescuentoGeneral    { get; set; }


    public decimal calcularTotal(){
        decimal total = 0;
        if(Articulos != null){
            foreach(var articulo in Articulos){
                total += articulo.PrecioUnitario * articulo.Cantidad - (articulo.PrecioUnitario * articulo.Cantidad * (articulo.Descuento / 100));
            }
        }
        return total;
    }

public decimal calcularSubtotal()
{
    decimal subtotal = 0;

    if (Articulos != null)
    {
        foreach (var articulo in Articulos)
        {
            var precioConDesc = articulo.PrecioUnitario * (1 - articulo.Descuento / 100m);

            var precioSinIva = precioConDesc / 1.21m;

            subtotal += precioSinIva * articulo.Cantidad;
        }
    }

    return subtotal;
}

public decimal calcularIva(){
    return calcularTotal() - calcularSubtotal();
}

public decimal CalcularDescuento()
{
    decimal totalSinDescuentos = 0;
    decimal totalConDescuentoPorItem = 0;
    decimal totalFinalConDescuentos = 0;

    if (Articulos != null)
    {
        foreach (var articulo in Articulos)
        {
            var precioBase = articulo.PrecioUnitario * articulo.Cantidad;

            totalSinDescuentos += precioBase;

            var precioConDescItem = precioBase * (1 - articulo.Descuento / 100m);

            totalConDescuentoPorItem += precioConDescItem;
        }
    }

    if (DescuentoGeneral.HasValue && DescuentoGeneral.Value > 0)
    {
        totalFinalConDescuentos = totalConDescuentoPorItem * (1 - DescuentoGeneral.Value / 100m);
    }
    else
    {
        totalFinalConDescuentos = totalConDescuentoPorItem;
    }
    return totalSinDescuentos - totalFinalConDescuentos;
}


    }

