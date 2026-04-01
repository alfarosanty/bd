using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EstadoPedidoProduccionConfiguration : IEntityTypeConfiguration<EstadoPedidoProduccion>
{
    public void Configure(EntityTypeBuilder<EstadoPedidoProduccion> builder)
    {
        builder.ToTable(EstadoPedidoProduccion.TABLA);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_ESTADO_PRODUCCION");

        builder.Property(x => x.Codigo)
               .HasColumnName("CODIGO")
               .HasMaxLength(20);

        builder.Property(x => x.Descripcion)
               .HasColumnName("DESCRIPCION")
               .HasMaxLength(100);
    }
}