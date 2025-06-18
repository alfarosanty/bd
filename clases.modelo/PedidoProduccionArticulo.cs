using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    public  class PedidoProduccionArticulo
        {
        public static String TABLA="PRODUCCION_ARTICULO";        
        public Articulo Articulo { get; set; }

        public int? IdPedidoProduccion{ get; set; }

        public int Cantidad { get; set; }

        /**Diferencia entrega la cantidad pedida y lo que devolvieron*/
        public int? cantidadPendiente { get; set; }

        public bool hayStock {get; set; }

        public string? descripcion { get;set; }

        public string codigo { get; set; }




    }
