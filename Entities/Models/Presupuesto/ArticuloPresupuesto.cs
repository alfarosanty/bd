using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


 public class ArticuloPresupuesto : IArticuloConStock
    {
    public static String TABLA="ARTICULO_PRESUPUESTO";

    public int Id { get; set; }
    public DateTime? FechaCreacion { get; set; }

    public int IdPresupuesto { get; set; }
    
    [JsonIgnore]
    public Presupuesto? Presupuesto { get; set; }

    public int IdArticulo { get; set; }
    public Articulo? Articulo { get; set; }

    public int Cantidad { get; set; }
    public int? CantidadPendiente { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public string? Descripcion { get; set; }
    public string? Codigo { get; set; }
    public bool HayStock { get; set; }
            
}