public class EstadisticaArticuloDTO{
    public int IdArticuloPrecio { get; set; }
    public string Codigo { get; set; }
    // Mapa: "Azul" -> 150.5, "Rojo" -> 40.0
    public Dictionary<string, decimal> CantidadPorColor { get; set; } = new();
    public decimal TotalGeneral { get; set; }
}