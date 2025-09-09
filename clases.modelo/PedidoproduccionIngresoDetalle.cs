    public  class PedidoProduccionIngresoDetalle
        {
        public static String TABLA="PEDIDO_PRODUCCION_INGRESO_DETALLE";
        public PedidoProduccion PedidoProduccion { get; set; }

        public Ingreso Ingreso { get; set; }


        public Presupuesto? Presupuesto { get; set; }

        public Articulo Articulo { get; set; }
                
        public int CantidadDescontada { get; set; }


    }
