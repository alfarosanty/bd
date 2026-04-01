using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PedidoProduccionConfiguration : IEntityTypeConfiguration<PedidoProduccion>
{
    public void Configure(EntityTypeBuilder<PedidoProduccion> builder)
    {
        builder.ToTable(PedidoProduccion.TABLA);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_PEDIDO_PRODUCCION");

        builder.Property(x => x.Fecha)
               .HasColumnName("FECHA")
               .HasColumnType("date");

        builder.Property(x => x.IdEstadoPedidoProduccion)
               .HasColumnName("ID_ESTADO_PEDIDO_PROD");
              
       builder.Property(x => x.IdTaller)
               .HasColumnName("ID_FABRICANTE");

        builder.Property(x => x.IDPresupuesto).HasColumnName("ID_PRESUPUESTO");

        // --- RELACIONES ---

       builder.HasOne(x => x.EstadoPedidoProduccion) 
               .WithMany()
               .HasForeignKey(x => x.IdEstadoPedidoProduccion);

        builder.HasOne(x => x.Taller)
               .WithMany()
               .HasForeignKey(x => x.IdTaller); 

        builder.HasMany(x => x.Articulos)
               .WithOne()
               .HasForeignKey(x => x.IdPedidoProduccion);
    }
}