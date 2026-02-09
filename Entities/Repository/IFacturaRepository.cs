/*public interface IFacturaRepository
{
    // ===== Escritura (EF) =====
    Task<int> CreateAsync(FacturaEntity factura);
    Task<FacturaEntity?> GetEntityByIdAsync(int idFactura);

    // ===== Lectura (Dapper) =====
    Task<FacturaDTO?> GetDTOByIdAsync(int idFactura);
    Task<List<FacturaListadoDTO>> GetByClienteAsync(int idCliente, DateTime? desde, DateTime? hasta);
}
*/