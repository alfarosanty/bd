using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


public class ArticuloFactura : IArticuloConStock
{
    public int IdArticuloFactura { get; set; }
    public int IdFactura { get; set; }
    public int IdArticulo { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string Codigo { get; set; }
    public string Descripcion { get; set; }
    public decimal Descuento { get; set; }

    // Usar 'virtual' evita errores de proxy en EF Core
    public virtual Articulo? Articulo { get; set; }

    [JsonIgnore]
    public virtual Factura? Factura { get; set; }
    
    [JsonIgnore]
    public DateTime Fecha { get; set; }
}

