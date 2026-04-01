using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PedidoProduccionArticuloConfiguration : IEntityTypeConfiguration<PedidoProduccionArticulo>
{
    public void Configure(EntityTypeBuilder<PedidoProduccionArticulo> builder)
    {
        builder.ToTable(PedidoProduccionArticulo.TABLA);

        builder.HasKey(x => x.IdProduccionArticulo);

        builder.Property(x => x.IdProduccionArticulo).HasColumnName("ID_PRODUCCION_ARTICULO");
        builder.Property(x => x.IdPedidoProduccion).HasColumnName("ID_PEDIDO_PRODUCCION");
        builder.Property(x => x.IdArticulo).HasColumnName("ID_ARTICULO");
        builder.Property(x => x.Codigo).HasColumnName("CODIGO").HasMaxLength(50);
        builder.Property(x => x.Cantidad).HasColumnName("CANTIDAD");
        builder.Property(x => x.CantidadPendiente).HasColumnName("CANT_PENDIENTE");
        builder.Property(x => x.Descripcion).HasColumnName("DESCRIPCION").HasMaxLength(250);

        builder.HasOne(x => x.Articulo)
               .WithMany()
               .HasForeignKey(x => x.IdArticulo);

        builder.Ignore(x => x.HayStock);
    }
}