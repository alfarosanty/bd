using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PedidoProduccionIngresoDetalleConfiguration : IEntityTypeConfiguration<PedidoProduccionIngresoDetalle>
{
    public void Configure(EntityTypeBuilder<PedidoProduccionIngresoDetalle> builder)
    {
        builder.ToTable(PedidoProduccionIngresoDetalle.TABLA);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("ID_DETALLE")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.IdPedidoProduccion).HasColumnName("ID_PEDIDO_PRODUCCION").IsRequired();
        builder.Property(p => p.IdIngreso).HasColumnName("ID_INGRESO").IsRequired();
        builder.Property(p => p.IdPresupuesto).HasColumnName("ID_PRESUPUESTO");
        builder.Property(p => p.IdArticulo).HasColumnName("ID_ARTICULO").IsRequired();

        builder.Property(p => p.CantidadDescontada).HasColumnName("CANTIDAD_DESCONTADA");
        builder.Property(p => p.CantidadPendienteAntes).HasColumnName("CANT_PENDIENTE_ANTES");
        builder.Property(p => p.CantidadPendienteDespues).HasColumnName("CANT_PENDIENTE_DESPUES");

        // Relaciones
        builder.HasOne(p => p.PedidoProduccion)
            .WithMany()
            .HasForeignKey(p => p.IdPedidoProduccion)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Ingreso)
            .WithMany()
            .HasForeignKey(p => p.IdIngreso)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Articulo)
            .WithMany()
            .HasForeignKey(p => p.IdArticulo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}