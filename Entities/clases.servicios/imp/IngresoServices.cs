

using BlumeAPI.Entities.clases.modelo;
using BlumeAPI.Services.Imp;
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

public List<Ingreso> GetIngresoByTaller(int idTaller, NpgsqlConnection conex)
{
    List<Ingreso> ingresos = new List<Ingreso>();
    string commandText = getSelect() + GetFromText() + " WHERE I.\"ID_FABRICANTE\" = @id";

    using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conex))
    {
        Console.WriteLine("Consulta: " + commandText);
        cmd.Parameters.AddWithValue("id", idTaller);

        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                ingresos.Add(ReadIngreso(reader, conex));
            }
        }
    }

    // ===== Aquí llenamos los artículos de cada ingreso =====
    foreach (var ingreso in ingresos)
    {
        ingreso.Articulos = getArticulosIngreso(ingreso, conex);
    }

    return ingresos.OrderBy(i=>i.Id).ToList();
}


public List<Ingreso> GetIngresosByIds(List<int> idsIngresos, NpgsqlConnection conex)
{
    var ingresos = new List<Ingreso>();

    foreach (var id in idsIngresos)
    {
        var ingreso = getIngreso(id, conex); // tu método que devuelve un Ingreso
        if (ingreso != null)
        {
            ingresos.Add(ingreso);
        }
    }

    return ingresos.OrderBy(i=>i.Id).ToList();
}



        public  int crear(Ingreso ingreso, Npgsql.NpgsqlConnection npgsqlConnection)
        {
            //OBTENGO EL ID DEL PEDIDO PRODUCCION
            string sqlSeq = "select nextval('\"INGRESO_ID_INGRESO_seq\"')";
            NpgsqlCommand cmdSeq = new NpgsqlCommand(sqlSeq, npgsqlConnection);
            Console.WriteLine("Ingreso el  " + Ingreso.TABLA  + " el remito ingreso" + sqlSeq);
            int idIngreso =   Convert.ToInt32(cmdSeq.ExecuteScalar()) ;         
            //CREO EL INSERT EN LA TABLA PRESUPUESTO
            string sqlInsert = "INSERT INTO  \""+ Ingreso.TABLA + "\" (\"ID_INGRESO\",\"FECHA_INGRESO\",\"ID_FABRICANTE\") VALUES(@ID_INGRESO,@FECHA_INGRESO,@ID_FABRICANTE)";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);            
            cmd.Parameters.AddWithValue("ID_INGRESO",idIngreso);
            cmd.Parameters.AddWithValue("FECHA_INGRESO",ingreso.Fecha);
            cmd.Parameters.AddWithValue("ID_FABRICANTE",ingreso.taller.Id);
             cmd.ExecuteNonQuery();                
            //RECORRO Y GUARDO LOS PRESUPUESTOS
            if(ingreso.Articulos !=null)
                foreach(ArticuloIngreso ia in ingreso.Articulos){
            sqlInsert = "INSERT INTO \"" + ArticuloIngreso.TABLA + "\" (\"ID_ARTICULO\",\"ID_INGRESO\",\"CANTIDAD\",\"FECHA_INGRESO\",\"CODIGO\",\"DESCRIPCION\") " + "VALUES(@ID_ARTICULO,@ID_INGRESO,@CANTIDAD,@FECHA_INGRESO,@CODIGO,@DESCRIPCION) ";
                        cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
                        {                        
                            cmd.Parameters.AddWithValue("ID_ARTICULO",ia.Articulo.Id);
                            cmd.Parameters.AddWithValue("ID_INGRESO",idIngreso);
                            cmd.Parameters.AddWithValue("CANTIDAD",ia.cantidad);
                            cmd.Parameters.AddWithValue("FECHA_INGRESO",ingreso.Fecha);
                            cmd.Parameters.AddWithValue("CODIGO",ia.Codigo);
                            cmd.Parameters.AddWithValue("DESCRIPCION",ia.Descripcion);
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Ingreso el  " + Ingreso.TABLA +   " el aritculo" + ia.Articulo.Id);
                        }
                }
                 actualizarStock(ingreso.Articulos, npgsqlConnection);

                
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
                               "(\"ID_ARTICULO\", \"ID_INGRESO\", \"CANTIDAD\", \"FECHA_INGRESO\") " +
                               "VALUES(@ID_ARTICULO, @ID_INGRESO, @CANTIDAD, @FECHA_INGRESO)";
            NpgsqlCommand cmdInsert = new NpgsqlCommand(sqlInsert, npgsqlConnection);
            cmdInsert.Parameters.AddWithValue("ID_ARTICULO", ia.Articulo.Id);
            cmdInsert.Parameters.AddWithValue("ID_INGRESO", ingreso.Id);  // Usa el mismo ID del presupuesto existente
            cmdInsert.Parameters.AddWithValue("CANTIDAD", ia.cantidad);
            cmdInsert.Parameters.AddWithValue("FECHA_INGRESO", ingreso.Fecha);
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


public int BorrarIngreso(Ingreso ingreso, NpgsqlConnection npgsqlConnection)
{
    if (ingreso == null)
        throw new ArgumentNullException(nameof(ingreso));

    using var transaction = npgsqlConnection.BeginTransaction();

    try
    {
        int idIngreso = ingreso.Id;

        // 1️⃣ Obtener IDs de Pedido Producción asociados (ANTES de borrar)
        List<int> pedidosProduccion = new();

        string sqlGetPP = @"
            SELECT DISTINCT ""ID_PEDIDO_PRODUCCION""
            FROM ""PEDIDO_PRODUCCION_INGRESO_DETALLE""
            WHERE ""ID_INGRESO"" = @ID_INGRESO;";

        using (var cmd = new NpgsqlCommand(sqlGetPP, npgsqlConnection, transaction))
        {
            cmd.Parameters.AddWithValue("ID_INGRESO", idIngreso);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                pedidosProduccion.Add(reader.GetInt32(0));
        }

        // 2️⃣ Disminuir stock
        if (ingreso.Articulos != null && ingreso.Articulos.Count > 0)
        {
            disminuirStock(ingreso.Articulos, npgsqlConnection);
        }

        // 3️⃣ Borrar ARTICULO_INGRESO
        string sqlDeleteArticulos = @"
            DELETE FROM ""ARTICULO_INGRESO""
            WHERE ""ID_INGRESO"" = @ID_INGRESO;";

        using (var cmd = new NpgsqlCommand(sqlDeleteArticulos, npgsqlConnection, transaction))
        {
            cmd.Parameters.AddWithValue("ID_INGRESO", idIngreso);
            cmd.ExecuteNonQuery();
        }

        // 4️⃣ Borrar PEDIDO_PRODUCCION_INGRESO_DETALLE
        string sqlDeleteDetalles = @"
            DELETE FROM ""PEDIDO_PRODUCCION_INGRESO_DETALLE""
            WHERE ""ID_INGRESO"" = @ID_INGRESO;";

        using (var cmd = new NpgsqlCommand(sqlDeleteDetalles, npgsqlConnection, transaction))
        {
            cmd.Parameters.AddWithValue("ID_INGRESO", idIngreso);
            cmd.ExecuteNonQuery();
        }

        // 5️⃣ Borrar INGRESO
        string sqlDeleteIngreso = @"
            DELETE FROM ""INGRESO""
            WHERE ""ID_INGRESO"" = @ID_INGRESO
            RETURNING ""ID_INGRESO"";";

        using (var cmd = new NpgsqlCommand(sqlDeleteIngreso, npgsqlConnection, transaction))
        {
            idIngreso = Convert.ToInt32(cmd.ExecuteScalar());
        }

        // 6️⃣ Actualizar estado de los Pedidos Producción
        if (pedidosProduccion.Count > 0)
        {
            string sqlUpdatePP = @"
                UPDATE ""PEDIDO_PRODUCCION""
                SET ""ESTADO"" = (
                    SELECT ""ID_ESTADO_PRODUCCION""
                    FROM ""ESTADO_PEDIDO_PRODUCCION""
                    WHERE ""DESCRIPCION"" = 'TALLER'
                )
                WHERE ""ID_PEDIDO_PRODUCCION"" = ANY(@ids);";

            using var cmd = new NpgsqlCommand(sqlUpdatePP, npgsqlConnection, transaction);
            cmd.Parameters.AddWithValue("ids", pedidosProduccion);
            cmd.ExecuteNonQuery();
        }

        transaction.Commit();
        return idIngreso;
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}



public List<PedidoProduccionIngresoDetalle> GetDetallesPPI(int idIngreso, NpgsqlConnection conex)
{
    var detalles = new List<PedidoProduccionIngresoDetalle>();

    string sqlSelect = @"
        SELECT 
            PPI.""ID_DETALLE"", 
            PPI.""ID_PEDIDO_PRODUCCION"", 
            PPI.""ID_INGRESO"", 
            PPI.""ID_PRESUPUESTO"", 
            PPI.""ID_ARTICULO"", 
            PPI.""CANTIDAD_DESCONTADA"",
            PPI.""CANTIDAD_PENDIENTE_ANTES"",
            PPI.""CANTIDAD_PENDIENTE_DESPUES"",
            COALESCE(CLI.""RAZON_SOCIAL"", 'Stock') AS ""RAZON_SOCIAL"",
            A.""CODIGO"" AS ""CODIGO_ARTICULO"",
            A.""DESCRIPCION"" AS ""DESCRIPCION_ARTICULO"",
            COL.""CODIGO"" AS ""CODIGO_COLOR""
        FROM public.""PEDIDO_PRODUCCION_INGRESO_DETALLE"" PPI
        LEFT JOIN public.""PRESUPUESTO"" P ON P.""ID_PRESUPUESTO"" = PPI.""ID_PRESUPUESTO""
        LEFT JOIN public.""CLIENTE"" CLI ON CLI.""ID_CLIENTE"" = P.""ID_CLIENTE""
        JOIN public.""ARTICULO"" A ON A.""ID_ARTICULO"" = PPI.""ID_ARTICULO""
        JOIN public.""COLOR"" COL ON COL.""ID_COLOR"" = A.""ID_COLOR""
        WHERE PPI.""ID_INGRESO"" = @idIngreso
        ORDER BY PPI.""ID_DETALLE"" ASC;
    ";

    using (var cmd = new NpgsqlCommand(sqlSelect, conex))
    {
        cmd.Parameters.AddWithValue("idIngreso", idIngreso);

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var detalle = new PedidoProduccionIngresoDetalle
                {
                    PedidoProduccion = new PedidoProduccion
                    {
                        Id = (int)reader["ID_PEDIDO_PRODUCCION"]
                    },
                    Ingreso = new Ingreso
                    {
                        Id = (int)reader["ID_INGRESO"]
                    },
                    Articulo = new Articulo
                    {
                        Id = (int)reader["ID_ARTICULO"],
                        Codigo = reader["CODIGO_ARTICULO"].ToString(),
                        Descripcion = reader["DESCRIPCION_ARTICULO"].ToString(),
                        Color = new Color
                        {
                            Codigo = reader["CODIGO_COLOR"].ToString()
                        }
                    },
                    CantidadDescontada = reader["CANTIDAD_DESCONTADA"] != DBNull.Value ? (int)reader["CANTIDAD_DESCONTADA"] : 0,
                    CantidadPendienteAntes = reader["CANTIDAD_PENDIENTE_ANTES"] != DBNull.Value ? (int)reader["CANTIDAD_PENDIENTE_ANTES"] : 0,
                    CantidadPendienteDespues = reader["CANTIDAD_PENDIENTE_DESPUES"] != DBNull.Value ? (int)reader["CANTIDAD_PENDIENTE_DESPUES"] : 0
                };

                // Presupuesto (puede ser nulo)
                if (reader["ID_PRESUPUESTO"] != DBNull.Value)
                {
                    detalle.Presupuesto = new Presupuesto
                    {
                        Id = (int)reader["ID_PRESUPUESTO"],
                        Cliente = new Cliente
                        {
                            RazonSocial = reader["RAZON_SOCIAL"].ToString()
                        }
                    };
                }
                else
                {
                    detalle.Presupuesto = null;
                }

                detalles.Add(detalle);
            }
        }
    }

    return detalles;
}




public List<int> CrearDetallesIngresoPedidoProduccion(List<PedidoProduccionIngresoDetalle> detalles, NpgsqlConnection npgsqlConnection)
{
    List<int> idsCreados = new List<int>();

    foreach (var detalle in detalles)
    {
        try
        {
            string sqlInsert = $@"
            INSERT INTO public.{PedidoProduccionIngresoDetalle.TABLA}
            (""ID_PEDIDO_PRODUCCION"", ""ID_INGRESO"", ""ID_PRESUPUESTO"", ""ID_ARTICULO"", ""CANTIDAD_DESCONTADA"", ""CANTIDAD_PENDIENTE_ANTES"", ""CANTIDAD_PENDIENTE_DESPUES"")
            VALUES (@ID_PEDIDO_PRODUCCION, @ID_INGRESO, @ID_PRESUPUESTO, @ID_ARTICULO, @CANTIDAD_DESCONTADA, @CANTIDAD_PENDIENTE_ANTES, @CANTIDAD_PENDIENTE_DESPUES)
            RETURNING ""ID_DETALLE"";";

            Console.WriteLine("SQL generado:");
            Console.WriteLine(sqlInsert);

            using var cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);

            Console.WriteLine($"-> ID_PEDIDO_PRODUCCION = {detalle.PedidoProduccion?.Id}");
            Console.WriteLine($"-> ID_INGRESO = {detalle.Ingreso?.Id}");
            Console.WriteLine($"-> ID_PRESUPUESTO = {(detalle.Presupuesto != null ? detalle.Presupuesto.Id.ToString() : "NULL")}");
            Console.WriteLine($"-> ID_ARTICULO = {detalle.Articulo?.Id}");
            Console.WriteLine($"-> CANTIDAD_DESCONTADA = {detalle.CantidadDescontada}");

            cmd.Parameters.AddWithValue("ID_PEDIDO_PRODUCCION", detalle.PedidoProduccion.Id);
            cmd.Parameters.AddWithValue("ID_INGRESO", detalle.Ingreso.Id);
            cmd.Parameters.AddWithValue("ID_PRESUPUESTO", (detalle.Presupuesto == null || detalle.Presupuesto.Id == 0) ? (object)DBNull.Value : detalle.Presupuesto.Id);
            cmd.Parameters.AddWithValue("ID_ARTICULO", detalle.Articulo.Id);
            cmd.Parameters.AddWithValue("CANTIDAD_DESCONTADA", detalle.CantidadDescontada);
            cmd.Parameters.AddWithValue("CANTIDAD_PENDIENTE_ANTES", detalle.CantidadPendienteAntes);
            cmd.Parameters.AddWithValue("CANTIDAD_PENDIENTE_DESPUES", detalle.CantidadPendienteDespues);


            var result = cmd.ExecuteScalar();
            Console.WriteLine($"ExecuteScalar result = {result}");

            int idCreado = Convert.ToInt32(result);
            idsCreados.Add(idCreado);

            Console.WriteLine($"✅ Insert realizado. ID_DETALLE = {idCreado}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error insertando detalle: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw; // re-lanzo para no ocultar el error
        }
    }

    Console.WriteLine($"Total inserts realizados: {idsCreados.Count}");
    return idsCreados;
}




private static Ingreso ReadIngreso(NpgsqlDataReader reader,NpgsqlConnection conex){
       int id =(int) reader["ID_INGRESO"];
       DateTime fecha = (DateTime) reader["FECHA_INGRESO"];
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
        DateTime fecha =(DateTime)  reader["FECHA_INGRESO"];
        string codigo =(string) reader["CODIGO"];
        string descripcion =(string) reader["DESCRIPCION"];
        CConexion cConexion= new CConexion();
        NpgsqlConnection conex2 =  cConexion.establecerConexion();
        Articulo articulo = new ArticuloServices().GetArticulo(idArticulo, conex2);
        cConexion.cerrarConexion(conex2);
            return new ArticuloIngreso{
                Articulo = articulo,
                //Presupuesto = presupuesto,
                cantidad = cantidadI,
                IdIngreso = ingreso.Id,
                Codigo = codigo,
                Descripcion = descripcion
                };


    }

private static void actualizarStock(List<ArticuloIngreso> ingresoArticulos, NpgsqlConnection conex)
{
string updateQuery = @"
    UPDATE """ + Articulo.TABLA + @"""
    SET ""STOCK"" = COALESCE(""STOCK"", 0) + @CANTIDAD
    WHERE ""ID_ARTICULO"" = @ID_ARTICULO;
";


    using (var cmd = new NpgsqlCommand(updateQuery, conex))
    {
        foreach (var ia in ingresoArticulos)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CANTIDAD", ia.cantidad);
            cmd.Parameters.AddWithValue("@ID_ARTICULO", ia.Articulo.Id);

            cmd.ExecuteNonQuery();
        }
    }
}

private static void disminuirStock(List<ArticuloIngreso> ingresoArticulos, NpgsqlConnection conex)
{
string updateQuery = @"
    UPDATE """ + Articulo.TABLA + @"""
    SET ""STOCK"" = COALESCE(""STOCK"", 0) - @CANTIDAD
    WHERE ""ID_ARTICULO"" = @ID_ARTICULO;
";


    using (var cmd = new NpgsqlCommand(updateQuery, conex))
    {
        foreach (var ia in ingresoArticulos)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CANTIDAD", ia.cantidad);
            cmd.Parameters.AddWithValue("@ID_ARTICULO", ia.Articulo.Id);

            cmd.ExecuteNonQuery();
        }
    }
}

}