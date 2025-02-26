

using Npgsql;

public class PresupuestoServices
{
    public  string getTabla()
    {
        return Presupuesto.TABLA;
    }




    public  Presupuesto readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }


     public Presupuesto GetPresupuesto(int id, NpgsqlConnection conex ){
            string commandText =  getSelect() + GetFromText()+ " WHERE PR.\"ID_PRESUPUESTO\" = @id";
            using(NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
               {
                 Console.WriteLine("Consulta: "+ commandText);
                    cmd.Parameters.AddWithValue("id", id);
                     using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            Presupuesto presu = ReadPresupeusto(reader);
                            return presu;
                            
                        }
                }
                return null;
                }

    

    
        public  void crear(Presupuesto presupuesto, Npgsql.NpgsqlConnection npgsqlConnection)
        {

            //OBTENGO EL ID DEL PRESUPUESTO
            string sqlSeq = "select nextval('\"PRESUPUESTO_ID_PRESUPUESTO_seq\"')";
            NpgsqlCommand cmdSeq = new NpgsqlCommand(sqlSeq, npgsqlConnection);
            Console.WriteLine("Ingreso el  " + Presupuesto.TABLA  + " el remito ingreso" + sqlSeq);
            int idPresupuesto =   Convert.ToInt32(cmdSeq.ExecuteScalar()) ;

            
            //CREO EL INSERT EN LA TABLA PRESUPUESTO
            string sqlInsert = "INSERT INTO  \""+ Presupuesto.TABLA + "\" (\"ID_PRESUPUESTO\",\"FECHA_PRESUPUESTO\",\"ID_CLIENTE\",\"EXMIR_IVA\",\"ID_ESTADO\") VALUES(@ID_PRESUPUESTO,@FECHA_PRESUPUESTO,@ID_CLIENTE,@EXMIR_IVA,@ID_ESTADO)";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);            
            cmd.Parameters.AddWithValue("ID_PRESUPUESTO",idPresupuesto);
            cmd.Parameters.AddWithValue("FECHA_PRESUPUESTO",presupuesto.Fecha);
            cmd.Parameters.AddWithValue("ID_CLIENTE",presupuesto.Cliente.Id);
            cmd.Parameters.AddWithValue("EXMIR_IVA",presupuesto.EximirIVA);
             cmd.Parameters.AddWithValue("ID_ESTADO",1);     
             cmd.ExecuteNonQuery();      
          
            //RECORRO Y GUARDO LOS PRESUPUESTOS
            foreach(ArticuloPresupuesto ap in presupuesto.Articulos){
                    sqlInsert = "INSERT INTO  \""+ ArticuloPresupuesto.TABLA + "\" (\"ID_ARTICULO\",\"ID_INGRESO\",\"CANTIDAD\",\"FECHA\") VALUES(@ID_ARTICULO,@ID_INGRESO,@CANTIDAD,@FECHA)";
                     cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
                    {                        
                        cmd.Parameters.AddWithValue("ID_PRESUPUESTO",idPresupuesto);
                        cmd.Parameters.AddWithValue("ID_ARTICULO",ap.Articulo.Id);
                        cmd.Parameters.AddWithValue("CANTIDAD",ap.cantidad);
                        cmd.Parameters.AddWithValue("FECHA",ap.PrecioUnitario);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Ingreso el  " + ArticuloPresupuesto.TABLA +   " el aritculo" + ap.Articulo.Id);
                    }
                    //ACTUALIZAR EL STOCK DE ESE ARTICULO
            }
            
            
        }






/**
            cmd.Parameters.AddWithValue("ID_CLIENTE",presupuesto.Cliente.Id);
            cmd.Parameters.AddWithValue("EXMIR_IVA",presupuesto.EximirIVA);
             cmd.Parameters.AddWithValue("ID_ESTADO",1);     
      **/       
private static Presupuesto ReadPresupeusto(NpgsqlDataReader reader){
        int? id = reader["ID_PRESUPUESTO"] as int?;
       // DateTime fecha = reader["FECHA_PRESUPUESTO"] as DateTime;
           return new Presupuesto{
             Id = id.Value
             //Fecha = fecha
             };


}
    private static string getSelect()
        {
            return  $"SELECT PR.* ";
            
        }
    private static string getSelectByArticulo()
    {
        return  $"SELECT PR.* ,AP.\"CANTIDAD\" AS CANTIDAD ";
        
    }
    private static string GetFromText()
    {
        
        return "FROM \"PRESUPUESTO\" PR";
    }

private static string GetFromTextByArticulo()
    {
        
        return "FROM \"PRESUPUESTO\" PR,\"ARTICULO_PRESUPUESTO\" AP ";
    }

   private static string GetWhereTextByArticulo()
    {
        return "WHERE AP.\"ID_PRESUPUESTO\" = AP.\"ID_PRESUPUESTO\" ";
    }


}