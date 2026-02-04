using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

public class ArticuloConfiguration : IEntityTypeConfiguration<ArticuloEntity>
{
    public void Configure(EntityTypeBuilder<ArticuloEntity> entity)
    {
        entity.ToTable("ARTICULO");

        // PK
        entity.HasKey(a => a.IdArticulo);

        // Columnas
        entity.Property(a => a.IdArticulo)
            .HasColumnName("ID_ARTICULO");

        entity.Property(a => a.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(a => a.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);

        entity.Property(a => a.IdColor)
            .HasColumnName("ID_COLOR");

        entity.Property(a => a.IdMedida)
            .HasColumnName("ID_MEDIDA");

        entity.Property(a => a.IdSubFamilia)
            .HasColumnName("ID_SUBFAMILIA");

        entity.Property(a => a.IdArticuloPrecio)
            .HasColumnName("ID_ARTICULO_PRECIO");

        entity.Property(a => a.IdFabricante)
            .HasColumnName("ID_FABRICANTE");

        entity.Property(a => a.Habilitado)
            .HasColumnName("HABILITADO");

        entity.Property(a => a.Stock)
            .HasColumnName("STOCK");
    }
}
