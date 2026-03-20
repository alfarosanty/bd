using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 public class ArticuloPresupuesto : IArticuloConStock
        {
            public static String TABLA="ARTICULO_PRESUPUESTO";

            public Articulo Articulo { get; set; }

            public int Cantidad { get; set; }
            public Presupuesto? Presupuesto { get; set; }
            public int? CantidadPendiente { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Descuento { get; set; }
            public string? descripcion { get; set; }
            public bool hayStock {get; set; }
            public string? codigo {get; set; }
            
    }