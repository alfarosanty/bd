

using Npgsql;

public class IngresoService
{
    public  string getTabla()
    {
        return Ingreso.TABLA;
    }




    public  Ingreso readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }

    public Ingreso getIngreso(int id, NpgsqlConnection conex ){
        Ingreso ingreso = new Ingreso();
            string commandText =  getSelect() + GetFromText()+ " WHERE I.\"ID_INGRESO\" = @id";
            using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", id);
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            ingreso = ReadIngreso(reader, conex);                           
                            
                        }
                }
                ingreso.Articulos =  getArticulosIngreso(ingreso,conex);
                return ingreso;
                }

    public List<Ingreso> GetIngresoByTaller(int idTaller, NpgsqlConnection conex ){
            List<Ingreso> ingresos = new List<Ingreso>();
            string commandText =  getSelect() + GetFromText()+ " WHERE I.\"ID_FABRICANTE\" = @id";
            using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", idTaller);
                     
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            ingresos.Add(ReadIngreso(reader, conex));
                            
                            
                        }
                }
                return ingresos;
        }
    

    
        public  int crear(Ingreso ingreso, Npgsql.NpgsqlConnection npgsqlConnection)
        {
            //OBTENGO EL ID DEL PEDIDO PRODUCCION
            string sqlSeq = "select nextval('\"INGRESO_ID_INGRESO_seq\"')";
            NpgsqlCommand cmdSeq = new NpgsqlCommand(sqlSeq, npgsqlConnection);
            Console.WriteLine("Ingreso el  " + Ingreso.TABLA  + " el remito ingreso" + sqlSeq);
            int idIngreso =   Convert.ToInt32(cmdSeq.ExecuteScalar()) ;         
            //CREO EL INSERT EN LA TABLA PRESUPUESTO
            string sqlInsert = "INSERT INTO  \""+ Ingreso.TABLA + "\" (\"ID_INGRESO\",\"FECHA\",\"ID_FABRICANTE\") VALUES(@ID_INGRESO,@FECHA,@ID_FABRICANTE)";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);            
            cmd.Parameters.AddWithValue("ID_INGRESO",idIngreso);
            cmd.Parameters.AddWithValue("FECHA",ingreso.Fecha);
            cmd.Parameters.AddWithValue("ID_FABRICANTE",ingreso.taller.Id);
             cmd.ExecuteNonQuery();                
            //RECORRO Y GUARDO LOS PRESUPUESTOS
            if(ingreso.Articulos !=null)
                foreach(ArticuloIngreso ia in ingreso.Articulos){
            sqlInsert = "INSERT INTO \"" + ArticuloIngreso.TABLA + "\" (\"ID_ARTICULO\",\"ID_INGRESO\",\"CANTIDAD\",\"FECHA\") " + "VALUES(@ID_ARTICULO,@ID_INGRESO,@CANTIDAD,@FECHA) ";
                        cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
                        {                        
                            cmd.Parameters.AddWithValue("ID_ARTICULO",ia.Articulo.Id);
                            cmd.Parameters.AddWithValue("ID_INGRESO",idIngreso);
                            cmd.Parameters.AddWithValue("CANTIDAD",ia.cantidad);
                            cmd.Parameters.AddWithValue("FECHA",ingreso.Fecha);
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Ingreso el  " + Ingreso.TABLA +   " el aritculo" + ia.Articulo.Id);
                        }
                }
                
                return idIngreso;
            }


        public int actualizar(Ingreso ingreso, Npgsql.NpgsqlConnection npgsqlConnection)
    {
        // Elimina los artículos antiguos asociados a este presupuesto
        string sqlDelete = "DELETE FROM \"" + ArticuloIngreso.TABLA + "\" WHERE \"ID_INGRESO\" = @ID_INGRESO";
        NpgsqlCommand cmdDelete = new NpgsqlCommand(sqlDelete, npgsqlConnection);
        cmdDelete.Parameters.AddWithValue("ID_INGRESO", ingreso.Id);
        cmdDelete.ExecuteNonQuery();

        // Ahora inserta los nuevos artículos
        if (ingreso.Articulos != null)
         {
        foreach (ArticuloIngreso ia in ingreso.Articulos)
        {
            string sqlInsert = "INSERT INTO \"" + ArticuloIngreso.TABLA + "\" " +
                               "(\"ID_ARTICULO\", \"ID_INGRESO\", \"CANTIDAD\", \"FECHA\") " +
                               "VALUES(@ID_ARTICULO, @ID_INGRESO, @CANTIDAD, @FECHA)";
            NpgsqlCommand cmdInsert = new NpgsqlCommand(sqlInsert, npgsqlConnection);
            cmdInsert.Parameters.AddWithValue("ID_ARTICULO", ia.Articulo.Id);
            cmdInsert.Parameters.AddWithValue("ID_INGRESO", ingreso.Id);  // Usa el mismo ID del presupuesto existente
            cmdInsert.Parameters.AddWithValue("CANTIDAD", ia.cantidad);
            cmdInsert.Parameters.AddWithValue("FECHA", ingreso.Fecha);
            cmdInsert.ExecuteNonQuery();
        }
    }

    // Actualiza el total en la tabla de presupuesto
    string sqlUpdateTotal = "UPDATE \"" + Ingreso.TABLA + "\" " +
                        "SET \"FECHA\" = @FECHA " + 
                        "WHERE \"ID_INGRESO\" = @ID_INGRESO";

    NpgsqlCommand cmdUpdateTotal = new NpgsqlCommand(sqlUpdateTotal, npgsqlConnection);
    cmdUpdateTotal.Parameters.AddWithValue("ID_INGRESO", ingreso.Id);
    cmdUpdateTotal.Parameters.AddWithValue("FECHA", ingreso.Fecha);
    cmdUpdateTotal.ExecuteNonQuery();  // Actualiza el total del presupuesto

    return ingreso.Id;  // Devuelve el mismo ID del presupuesto que fue actualizado
    }





private static Ingreso ReadIngreso(NpgsqlDataReader reader,NpgsqlConnection conex){
       int id =(int) reader["ID_INGRESO"];
       DateTime fecha = (DateTime) reader["FECHA"];
       int idFabricante = (int)reader["ID_FABRICANTE"];
      
       Ingreso ingreso = new Ingreso{
             Id = id,
             Fecha = fecha,
             };

            CConexion cconexio =  new CConexion();
          NpgsqlConnection conex2= cconexio.establecerConexion();

           ingreso.taller = new TallerServices().GetTaller(idFabricante, conex2);
          
           cconexio.cerrarConexion(conex2);
           return ingreso;


}
    private static string getSelect()
        {
            return  $"SELECT I.* ";
            
        }
    private static string getSelectByArticulo()
    {
        return  $"SELECT I.* ,AI.\"CANTIDAD\" AS CANTIDAD ";
        
    }
    private static string GetFromText()
    {
        
        return "FROM \"INGRESO\" I";
    }

private static string GetFromTextByArticulo()
    {
        
        return "FROM \"INGRESO\" I,\"ARTICULO_INGRESO\" AI ";
    }

   private static string GetWhereTextByArticulo()
    {
        return "WHERE AI.\"ID_INGRESO\" = AI.\"ID_INGRESO\" ";
    }


    private static List<ArticuloIngreso> getArticulosIngreso(Ingreso ingreso,NpgsqlConnection conex ){
        List<ArticuloIngreso> ingresoArticulos = new List<ArticuloIngreso>();
        string commandText = "SELECT AI.* FROM \"ARTICULO_INGRESO\" AI WHERE AI.\"ID_INGRESO\"=@IDINGRESO";
        using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("IDINGRESO", ingreso.Id);
                     
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            ingresoArticulos.Add(ReadArticuloIngreso(reader, ingreso,conex));
                            
                            
                        }
                }
                return ingresoArticulos;
    }
    private static ArticuloIngreso ReadArticuloIngreso(NpgsqlDataReader reader,Ingreso ingreso,NpgsqlConnection conex){
        int idArticulo =(int) reader["ID_ARTICULO"];
        int cantidadI =(int)  reader["CANTIDAD"] ;
        DateTime fecha =(DateTime)  reader["FECHA"];
        CConexion cConexion= new CConexion();
        NpgsqlConnection conex2 =  cConexion.establecerConexion();
        Articulo articulo = new ArticuloServices().GetArticulo(idArticulo, conex2);
        cConexion.cerrarConexion(conex2);
            return new ArticuloIngreso{
                Articulo = articulo,
                //Presupuesto = presupuesto,
                cantidad = cantidadI,
                IdIngreso = ingreso.Id
                };


    }

}