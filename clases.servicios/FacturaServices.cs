using Npgsql;

namespace BlumeAPI.Services;

public class FacturaServices
{

public  string getTabla()
    {
        return Presupuesto.TABLA;   
    }




    public  Presupuesto readBasico(NpgsqlDataReader reader)
    {
        throw new NotImplementedException();
    }


    public  int crear(Factura factura, Npgsql.NpgsqlConnection npgsqlConnection)
        {
            //OBTENGO EL ID DE LA FACTURA
            string sqlSeq = "select nextval('\"FACTURA_ID_FACTURA_seq\"')";
            NpgsqlCommand cmdSeq = new NpgsqlCommand(sqlSeq, npgsqlConnection);
            Console.WriteLine("Ingreso el  " + Factura.TABLA  + " el remito ingreso" + sqlSeq);
            int idFactura =   Convert.ToInt32(cmdSeq.ExecuteScalar()) ;         
            //CREO EL INSERT EN LA TABLA FACTURA
            string sqlInsert = "INSERT INTO  \""+ Factura.TABLA + "\" (\"ID_FACTURA\",\"FECHA\",\"ID_CLIENTE\",\"EXIMIR_IVA\",\"TOTAL\",\"ID_PRESUPUESTO\") VALUES(@ID_FACTURA,@FECHA,@ID_CLIENTE,@EXMIR_IVA,@TOTAL,@ID_PRESUPUESTO)";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);            
            cmd.Parameters.AddWithValue("ID_FACTURA",idFactura);
            cmd.Parameters.AddWithValue("FECHA",factura.Fecha);
            cmd.Parameters.AddWithValue("ID_CLIENTE",factura.Cliente.Id);
            cmd.Parameters.AddWithValue("EXMIR_IVA",factura.EximirIVA);
            cmd.Parameters.AddWithValue("TOTAL",1);
            cmd.Parameters.AddWithValue("ID_PRESUPUESTO", 2);
            cmd.ExecuteNonQuery();                
            //RECORRO Y GUARDO LAS FACTURAS
            /*
            if(presupuesto.Articulos !=null)
                foreach(ArticuloPresupuesto ap in presupuesto.Articulos){
                        sqlInsert = "INSERT INTO  \""+ ArticuloPresupuesto.TABLA + "\" (\"ID_ARTICULO\",\"ID_PRESUPUESTO\",\"CANTIDAD\",\"PRECIO_UNITARIO\") VALUES(@ID_ARTICULO,@ID_PRESUPUESTO,@CANTIDAD,@PRECIO_UNITARIO)";
                        cmd = new NpgsqlCommand(sqlInsert, npgsqlConnection);
                        {                        
                            cmd.Parameters.AddWithValue("ID_PRESUPUESTO",idPresupuesto);
                            cmd.Parameters.AddWithValue("ID_ARTICULO",ap.Articulo.Id);
                            cmd.Parameters.AddWithValue("CANTIDAD",ap.cantidad);
                            cmd.Parameters.AddWithValue("PRECIO_UNITARIO",ap.PrecioUnitario);
                            cmd.Parameters.AddWithValue("DESCUENTO",ap.Descuento);
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Ingreso el  " + ArticuloPresupuesto.TABLA +   " el aritculo" + ap.Articulo.Id);
                        }
                        //ACTUALIZAR EL STOCK DE ESE ARTICULO
                } */
                
                return idFactura;
                }


    private decimal calcularPrecioFinal(List<ArticuloFactura> articulosFacturas)
{
    decimal precioConDescuento = 0;

    foreach (var articuloFactura in articulosFacturas)
    {
        decimal descuento = articuloFactura.Descuento; // Asume 0 si es nulo
        decimal precioDescontado = articuloFactura.PrecioUnitario - (articuloFactura.PrecioUnitario * (descuento * 0.01m));

        precioConDescuento += precioDescontado;  // Todo es decimal ahora
    }

    return precioConDescuento;
}



}