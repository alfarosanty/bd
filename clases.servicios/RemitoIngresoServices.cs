using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class RemitoIngresoServices
    {    
        public async void  crear(RemitoIngreso remito, Npgsql.NpgsqlConnection npgsqlConnection)
        {
            //guardar RemitoIngreso
            string sqlInsert = "INSERT INTO  \""+ RemitoIngreso.TABLA + "\" (\"FECHA\",\"ID_FABRICANTE\",\"DESCRIPCION\") VALUES(@FECHA,@FABRICANTE,@DESCRIPCION)";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
            
            //cmd.Parameters.AddWithValue("ID_INGRESO","1");
            cmd.Parameters.AddWithValue("FECHA",remito.Fecha);
            cmd.Parameters.AddWithValue("FABRICANTE",remito.Taller.Id);
            cmd.Parameters.AddWithValue("DESCRIPCION",remito.Descripcion);
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
            
        }

        public void modificar(RemitoIngreso remito){
            //Actualizar RemitoIngreso
            //Actualziar ArticuloIngreso
            //Guardar
        }


        public RemitoIngreso buscar(Taller taller, DateTime dateTime, Articulo articulo){


            return null;
        }


    }

