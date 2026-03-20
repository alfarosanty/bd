using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 public class ArticuloIngreso : IArticuloConStock
        {
            public static String TABLA="ARTICULO_INGRESO";

            public Articulo Articulo { get; set; }

            public int? IdIngreso { get; set; }

            public int Cantidad { get; set; }

            public string Codigo { get; set; }

            public string Descripcion { get; set; }
            public DateTime Fecha { get; set; }
    }