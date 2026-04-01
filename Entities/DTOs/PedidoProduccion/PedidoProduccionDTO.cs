public class PedidoProduccionDTO
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public Taller Taller { get; set; }
    public EstadoPedidoProduccion EstadoPedidoProduccion { get; set; }
    public string ClienteNombre { get; set; } 
    
    public int? IdPresupuesto { get; set; }
    public List<PedidoProduccionArticulo> Articulos { get; set; } = new();
}
