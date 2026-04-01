using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public  class PedidoProduccion
        {
        public static String TABLA="PEDIDO_PRODUCCION";
        public int Id { get; set; }
        
        public DateTime Fecha { get; set; }
        
        public int IdTaller { get; set; }
        public Taller Taller { get; set; }

        public int IdEstadoPedidoProduccion { get; set; }
        public EstadoPedidoProduccion EstadoPedidoProduccion { get; set; }

        public int? IDPresupuesto {get; set; }

        public List<PedidoProduccionArticulo> Articulos { get; set; }
        
    }
