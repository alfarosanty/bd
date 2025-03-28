using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 public class ArticuloIngreso
        {
            public static String TABLA="ARTICULO_INGRESO";

            public Articulo Articulo { get; set; }

            public Ingreso Ingreso { get; set; }

            public int cantidad { get; set; }


    }