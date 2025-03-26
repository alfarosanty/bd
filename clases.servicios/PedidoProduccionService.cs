

using Npgsql;

public class PedidoProduccionService
{
    public  string getTabla()
    {
        return PedidoProduccion.TABLA;
    }




    public  PedidoProduccion readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }

    public PedidoProduccion getPedidoProduccion(int id, NpgsqlConnection conex ){
        PedidoProduccion pedidoProduccion = new PedidoProduccion();
            string commandText =  getSelect() + GetFromText()+ " WHERE PR.\"ID_PEDIDO_PRODUCCION\" = @id";
            using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", id);
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            pedidoProduccion = ReadPedidoProduccion(reader, conex);                           
                            
                        }
                }
                pedidoProduccion.Articulos =  getArticulosPedidoProduccion(pedidoProduccion,conex);
                return pedidoProduccion;
                }

    public List<PedidoProduccion> GetPresupuestoByTaller(int idTaller, NpgsqlConnection conex ){
            List<PedidoProduccion> pedidosProduccion = new List<PedidoProduccion>();
            string commandText =  getSelect() + GetFromText()+ " WHERE PP.\"ID_FABRICANTE\" = @id";
            using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", idTaller);
                     
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            pedidosProduccion.Add(ReadPedidoProduccion(reader, conex));
                            
                            
                        }
                }
                return pedidosProduccion;
        }
    

    
        public  int crear(PedidoProduccion pedidoProduccion, Npgsql.NpgsqlConnection npgsqlConnection)
        {
            //OBTENGO EL ID DEL PEDIDO PRODUCCION
            string sqlSeq = "select nextval('\"PEDIDO_PRODUCCION_ID_seq\"')";
            NpgsqlCommand cmdSeq = new NpgsqlCommand(sqlSeq, npgsqlConnection);
            Console.WriteLine("Ingreso el  " + PedidoProduccion.TABLA  + " el remito ingreso" + sqlSeq);
            int idPedidoProduccion =   Convert.ToInt32(cmdSeq.ExecuteScalar()) ;         
            //CREO EL INSERT EN LA TABLA PRESUPUESTO
            string sqlInsert = "INSERT INTO  \""+ PedidoProduccion.TABLA + "\" (\"ID_PEDIDO_PRODUCCION\",\"FECHA\",\"ID_FABRICANTE\",\"ID_ESTADO_PEDIDO_PROD\") VALUES(@ID_PEDIDO_PRODUCCION,@FECHA,@ID_FABRICANTE,@ID_ESTADO_PEDIDO_PROD)";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);            
            cmd.Parameters.AddWithValue("ID_PEDIDO_PRODUCCION",idPedidoProduccion);
            cmd.Parameters.AddWithValue("FECHA",pedidoProduccion.Fecha);
            cmd.Parameters.AddWithValue("ID_FABRICANTE",pedidoProduccion.taller.Id);
            cmd.Parameters.AddWithValue("ID_ESTADO_PEDIDO_PROD",1); 
             cmd.ExecuteNonQuery();                
            //RECORRO Y GUARDO LOS PRESUPUESTOS
            if(pedidoProduccion.Articulos !=null)
                foreach(PedidoProduccionArticulo ppa in pedidoProduccion.Articulos){
                        sqlInsert = "INSERT INTO  \""+ PedidoProduccionArticulo.TABLA + "\" (\"ID_ARTICULO\",\"ID_PEDIDO_PRODUCCION\",\"CANTIDAD\",\"CANT_PENDIENTE\") VALUES(@ID_ARTICULO,@ID_PEDIDO_PRODUCCION,@CANTIDAD,@CANT_PENDIENTE)";
                        cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
                        {                        
                            cmd.Parameters.AddWithValue("ID_PEDIDO_PRODUCCION",idPedidoProduccion);
                            cmd.Parameters.AddWithValue("ID_ARTICULO",ppa.Articulo.Id);
                            cmd.Parameters.AddWithValue("CANTIDAD",ppa.Cantidad);
                            cmd.Parameters.AddWithValue("CANT_PENDIENTE",0);
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Ingreso el  " + PedidoProduccionArticulo.TABLA +   " el aritculo" + ppa.Articulo.Id);
                        }
                        //ACTUALIZAR EL STOCK DE ESE ARTICULO
                }
                
                return idPedidoProduccion;
            }

/*
        public int actualizar(Presupuesto presupuesto, Npgsql.NpgsqlConnection npgsqlConnection)
    {
        // Elimina los artículos antiguos asociados a este presupuesto
        string sqlDelete = "DELETE FROM \"" + ArticuloPresupuesto.TABLA + "\" WHERE \"ID_PRESUPUESTO\" = @ID_PRESUPUESTO";
        NpgsqlCommand cmdDelete = new NpgsqlCommand(sqlDelete, npgsqlConnection);
        cmdDelete.Parameters.AddWithValue("ID_PRESUPUESTO", presupuesto.Id);
        cmdDelete.ExecuteNonQuery();

        // Ahora inserta los nuevos artículos
        if (presupuesto.Articulos != null)
         {
        foreach (ArticuloPresupuesto ap in presupuesto.Articulos)
        {
            string sqlInsert = "INSERT INTO \"" + ArticuloPresupuesto.TABLA + "\" " +
                               "(\"ID_ARTICULO\", \"ID_PRESUPUESTO\", \"CANTIDAD\", \"PRECIO_UNITARIO\") " +
                               "VALUES(@ID_ARTICULO, @ID_PRESUPUESTO, @CANTIDAD, @PRECIO_UNITARIO)";
            NpgsqlCommand cmdInsert = new NpgsqlCommand(sqlInsert, npgsqlConnection);
            cmdInsert.Parameters.AddWithValue("ID_PRESUPUESTO", presupuesto.Id);  // Usa el mismo ID del presupuesto existente
            cmdInsert.Parameters.AddWithValue("ID_ARTICULO", ap.Articulo.Id);
            cmdInsert.Parameters.AddWithValue("CANTIDAD", ap.cantidad);
            cmdInsert.Parameters.AddWithValue("PRECIO_UNITARIO", ap.PrecioUnitario);
            cmdInsert.ExecuteNonQuery();
        }
    }

    // Actualiza el total en la tabla de presupuesto
    string sqlUpdateTotal = "UPDATE \"" + Presupuesto.TABLA + "\" " +
                            "SET \"TOTAL_PRESUPUESTO\" = @TOTAL_PRESUPUESTO, " +
                            "     \"FECHA_PRESUPUESTO\" = @FECHA_PRESUPUESTO " +
                            "WHERE \"ID_PRESUPUESTO\" = @ID_PRESUPUESTO";
    NpgsqlCommand cmdUpdateTotal = new NpgsqlCommand(sqlUpdateTotal, npgsqlConnection);
    cmdUpdateTotal.Parameters.AddWithValue("TOTAL_PRESUPUESTO", calcularTotal(presupuesto.Articulos));
    cmdUpdateTotal.Parameters.AddWithValue("ID_PRESUPUESTO", presupuesto.Id);
    cmdUpdateTotal.Parameters.AddWithValue("FECHA_PRESUPUESTO", presupuesto.Fecha);
    cmdUpdateTotal.ExecuteNonQuery();  // Actualiza el total del presupuesto

    return presupuesto.Id;  // Devuelve el mismo ID del presupuesto que fue actualizado
    }

*/



private static PedidoProduccion ReadPedidoProduccion(NpgsqlDataReader reader,NpgsqlConnection conex){
       int id =(int) reader["ID_PEDIDO_PRODUCCION"];
       DateTime fecha = (DateTime) reader["FECHA"];
       int idFabricante = (int)reader["ID_FABRICANTE"];
       int idEstadoPedidoProduccion =(int) reader["ID_ESTADO_PEDIDO_PROD"];
      
       PedidoProduccion pedidoProduccion = new PedidoProduccion{
             Id = id,
             Fecha = fecha,
             IdEstadoPedidoProduccion = idEstadoPedidoProduccion
             };

           CConexion cconexio =  new CConexion();
          NpgsqlConnection conex2= cconexio.establecerConexion();

           pedidoProduccion.taller = new TallerServices().GetTaller(idFabricante, conex2);
          
           cconexio.cerrarConexion(conex2);
           return pedidoProduccion;


}
    private static string getSelect()
        {
            return  $"SELECT PP.* ";
            
        }
    private static string getSelectByArticulo()
    {
        return  $"SELECT PP.* ,PA.\"CANTIDAD\" AS CANTIDAD ";
        
    }
    private static string GetFromText()
    {
        
        return "FROM \"PEDIDO_PRODUCCION\" PP";
    }

private static string GetFromTextByArticulo()
    {
        
        return "FROM \"PEDIDO_PRODUCCION\" PP,\"PRODUCCION_ARTICULO\" PA ";
    }

   private static string GetWhereTextByArticulo()
    {
        return "WHERE PA.\"ID_PEDIDO_PRODUCCION\" = PA.\"ID_PEDIDO_PRODUCCION\" ";
    }


    private static List<PedidoProduccionArticulo> getArticulosPedidoProduccion(PedidoProduccion pedidoProduccion,NpgsqlConnection conex ){
        List<PedidoProduccionArticulo> pedidoProduccionArticulos = new List<PedidoProduccionArticulo>();
        string commandText = "SELECT PA.* FROM \"PRODUCCION_ARTICULO\" PA WHERE PA.\"ID_PEDIDO_PRODUCCION\"=@IDPEDIDOPRODUCCION";
        using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("IDPEDIDOPRODUCCION", pedidoProduccion.Id);
                     
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            pedidoProduccionArticulos.Add(ReadArticuloPedidoProduccion(reader, pedidoProduccion,conex));
                            
                            
                        }
                }
                return pedidoProduccionArticulos;
    }
    private static PedidoProduccionArticulo ReadArticuloPedidoProduccion(NpgsqlDataReader reader,PedidoProduccion pedidoProduccion,NpgsqlConnection conex){
        int idArticulo =(int) reader["ID_ARTICULO"];
        int cantidadAPP =(int)  reader["CANTIDAD"] ;
        int cantidadPendienteAPP =(int)  reader["CANT_PENDIENTE"];
        CConexion cConexion= new CConexion();
        NpgsqlConnection conex2 =  cConexion.establecerConexion();
        Articulo articulo = new ArticuloServices().GetArticulo(idArticulo, conex2);
        cConexion.cerrarConexion(conex2);
            return new PedidoProduccionArticulo{
                Articulo = articulo,
                //Presupuesto = presupuesto,
                Cantidad = cantidadAPP,
                CantidadPendiente= cantidadPendienteAPP
                };


    }

}