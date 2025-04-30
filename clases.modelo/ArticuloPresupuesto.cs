using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 public class ArticuloPresupuesto
        {
            public static String TABLA="ARTICULO_PRESUPUESTO";

            public Articulo Articulo { get; set; }

            public Presupuesto? Presupuesto { get; set; }

            public int cantidad { get; set; }

            public int? CantidadPendiente { get; set; }


            public decimal PrecioUnitario { get; set; }

            public decimal Descuento { get; set; }

            public string? descripcion { get; set; }

            public bool hayStock {get; set; }

            /**INCLUIR UN ESTAOD PARA SABER SI ESTA SEPARADO O SE MANDO A PRODUCCION**/

    }