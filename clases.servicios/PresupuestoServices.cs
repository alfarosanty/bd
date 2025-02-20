

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



    
        public async void  crear(Presupuesto presupuesto, Npgsql.NpgsqlConnection npgsqlConnection)
        {
            //guardar RemitoIngreso
            string sqlInsert = "INSERT INTO  \""+ Presupuesto.TABLA + "\" (\"FECHA_PRESUPUESTO\",\"ID_CLIENTE\",\"EXMIR_IVA\") VALUES(@FECHA_PRESUPUESTO,@ID_CLIENTE,@EXMIR_IVA,@EXMIR_IVA)";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
            
            //cmd.Parameters.AddWithValue("ID_INGRESO","1");
            cmd.Parameters.AddWithValue("FECHA_PRESUPUESTO",presupuesto.Fecha);
            cmd.Parameters.AddWithValue("ID_CLIENTE",presupuesto.Cliente.Id);
            cmd.Parameters.AddWithValue("EXMIR_IVA",presupuesto.EximirIVA);
            //cmd.Parameters.AddWithValue("ID_ESTADO",presupuesto.EstadoPresupuesto.id);
            //cmd.Parameters.AddWithValue("TOTAL_PRESUPUESTO",presupuesto.to.Descripcion);
            /*
            int nRows  = cmd.ExecuteNonQuery();
            string sqlSeq = "select currval('\"INGRESO_ID_INGRESO_seq\"')";
            NpgsqlCommand cmdSeq = new NpgsqlCommand(sqlSeq, npgsqlConnection);
            Console.WriteLine("Ingreso el  " + RemitoIngreso.TABLA  + " el remito ingreso" + sqlSeq);
             remito.Id =   Convert.ToInt32(cmdSeq.ExecuteScalar()) ;
            //
            
            //Guardar ArticuloIngreso
            //recorrer articulos e irlos guardando
                   foreach(ArticuloIngreso art in remito.Articulos ){
                    sqlInsert = "INSERT INTO  \""+ ArticuloIngreso.TABLA + "\" (\"ID_ARTICULO\",\"ID_INGRESO\",\"CANTIDAD\",\"FECHA\") VALUES(@ID_ARTICULO,@ID_INGRESO,@CANTIDAD,@FECHA)";
                     cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
                    {                        
                        cmd.Parameters.AddWithValue("ID_INGRESO",remito.Id);
                        cmd.Parameters.AddWithValue("ID_ARTICULO",art.Articulo.Id);
                        cmd.Parameters.AddWithValue("CANTIDAD",art.cantidad);
                        cmd.Parameters.AddWithValue("FECHA",remito.Fecha);
                         cmd.ExecuteNonQuery();
                        Console.WriteLine("Ingreso el  " + ArticuloIngreso.TABLA +   " el aritculo" + art.Articulo.Id);
                    }
                    //ACTUALIZAR EL STOCK DE ESE ARTICULO
            



            }
            */
            
        }


}