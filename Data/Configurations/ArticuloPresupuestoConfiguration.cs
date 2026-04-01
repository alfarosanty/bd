using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ArticuloPresupuestoConfiguration : IEntityTypeConfiguration<ArticuloPresupuesto>
{
    public void Configure(EntityTypeBuilder<ArticuloPresupuesto> builder)
    {
        builder.ToTable("ARTICULO_PRESUPUESTO");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_ARTICULO_PRESUPUESTO");

        builder.Property(x => x.FechaCreacion)
               .HasColumnName("FECHA_CREACION")
               .HasColumnType("date");

        builder.Property(x => x.IdPresupuesto).HasColumnName("ID_PRESUPUESTO");
        builder.Property(x => x.IdArticulo).HasColumnName("ID_ARTICULO");
        builder.Property(x => x.Cantidad).HasColumnName("CANTIDAD");
        builder.Property(x => x.CantidadPendiente).HasColumnName("PENDIENTE");
        builder.Property(x => x.PrecioUnitario).HasColumnName("PRECIO_UNITARIO").HasColumnType("numeric(18,2)");
        builder.Property(x => x.Descuento).HasColumnName("DESCUENTO").HasColumnType("numeric(18,2)");
        builder.Property(x => x.Descripcion).HasColumnName("DESCRIPCION");
        builder.Property(x => x.Codigo).HasColumnName("CODIGO");
        builder.Property(x => x.HayStock).HasColumnName("HAY_STOCK");
        builder.Ignore(x => x.Producir);

        builder.HasOne(x => x.Articulo)
               .WithMany()
               .HasForeignKey(x => x.IdArticulo);

        builder.HasOne(x => x.Presupuesto)
                .WithMany(p => p.Articulos)
                .HasForeignKey(x => x.IdPresupuesto);
    }
}