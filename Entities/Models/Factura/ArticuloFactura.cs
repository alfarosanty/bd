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
            public static String TABLA="ARTICULO_FACTURA";

            public int IdArticuloFactura { get; set; }
            public int IdFactura { get; set; }
            public int IdArticulo { get; set; }
            public int Cantidad { get; set; }

            public decimal PrecioUnitario { get; set; }
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
            public decimal Descuento { get; set; }

            public Articulo Articulo { get; set; }


            [JsonIgnore]
            public Factura? Factura { get; set; }
            [JsonIgnore]
            public DateTime Fecha { get; set; }

    }

