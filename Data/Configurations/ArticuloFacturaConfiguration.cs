using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlumeAPI.Data.Configurations
{
    public class ArticuloFacturaConfiguration : IEntityTypeConfiguration<ArticuloFactura>
    {
        public void Configure(EntityTypeBuilder<ArticuloFactura> entity)
        {
            entity.ToTable("ARTICULO_FACTURA");

            entity.HasKey(af => af.IdArticuloFactura);

            entity.Property(af => af.IdArticuloFactura)
                .HasColumnName("ID_ARTICULO_FACTURA");

            entity.Property(af => af.IdFactura)
                .HasColumnName("ID_FACTURA");

            entity.Property(af => af.IdArticulo)
                .HasColumnName("ID_ARTICULO");

            entity.Property(af => af.Cantidad)
                .HasColumnName("CANTIDAD");

            entity.Property(af => af.PrecioUnitario)
                .HasColumnName("PRECIO_UNITARIO");

            entity.Property(af => af.Codigo)
                .HasColumnName("CODIGO")
                .HasMaxLength(50);

            entity.Property(af => af.Descripcion)
                .HasColumnName("DESCRIPCION")
                .HasMaxLength(250);

            entity.Property(af => af.Descuento)
                .HasColumnName("DESCUENTO");

            // 🔗 Relación con Articulo
            entity.HasOne(af => af.Articulo)
                .WithMany()
                .HasForeignKey(af => af.IdArticulo)
                .HasConstraintName("ID_ARTICULO");
            
           entity.HasOne(af => af.Factura)
                .WithMany(f => f.Articulos)
                .HasForeignKey(af => af.IdFactura);

            //IGNORES
            entity.Ignore(af => af.Fecha);
        }
    }
}