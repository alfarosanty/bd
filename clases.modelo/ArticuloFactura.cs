using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 public class ArticuloFactura
        {
            public static String TABLA="ARTICULO_FACTURA";

            public Articulo Articulo { get; set; }

            public Factura Factura { get; set; }

            public int cantidad { get; set; }

            public decimal PrecioUnitario { get; set; }

    }