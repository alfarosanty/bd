using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    public  class PedidoProduccionArticulo : IArticuloConStock
        {
        public static String TABLA="PRODUCCION_ARTICULO";
        public int IdProduccionArticulo { get; set; }
        public int IdArticulo { get; set; }      
        public Articulo? Articulo { get; set; }

        public int? IdPedidoProduccion{ get; set; }

        public int Cantidad { get; set; }

        /**Diferencia entrega la cantidad pedida y lo que devolvieron*/
        public int? CantidadPendiente { get; set; }

        public bool HayStock {get; set; }

        public string? Descripcion { get;set; }

        public string Codigo { get; set; }

    }
