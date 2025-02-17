using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    public  class PedidoProduccionArticulo
        {
        public static String TABLA="PRODUCCION_ARTICULO";
        public int Id { get; set; }
        
        public Articulo Articulo { get; set; }

        public PedidoProduccion PedidoProduccion{ get; set; }

        public int Cantidad { get; set; }

        /**Diferencia entrega la cantidad pedida y lo que devolvieron*/
        public int CantidadPendiente { get; set; }



    }
