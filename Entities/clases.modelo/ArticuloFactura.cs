using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


 public class ArticuloFactura
        {
            public static String TABLA="ARTICULO_FACTURA";

            public int IdFactura { get; set; }


            public Articulo Articulo { get; set; }

            public int IdArticuloFactura { get; set; }

            [JsonIgnore]
            public Factura? Factura { get; set; }

            public int Cantidad { get; set; }

            public decimal PrecioUnitario { get; set; }

            public decimal Descuento { get; set; }

            public string Codigo { get; set; }

            public string Descripcion { get; set; }

    }

