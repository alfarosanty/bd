using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

namespace BlumeAPI.Data.Configurations
{
    public class ArticuloFacturaConfiguration : IEntityTypeConfiguration<ArticuloFacturaEntity>
    {
        public void Configure(EntityTypeBuilder<ArticuloFacturaEntity> entity)
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

            // 🔗 Relación con Factura (Muchos artículos pertenecen a una Factura)
            entity.HasOne(af => af.Factura)
                .WithMany(f => f.ArticulosFactura)
                .HasForeignKey(af => af.IdFactura);

            // 🔗 Relación con Articulo
            entity.HasOne(af => af.Articulo)
                .WithMany()
                .HasForeignKey(af => af.IdArticulo);
        }
    }
}