using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ArticuloPrecioConfiguration 
    : IEntityTypeConfiguration<ArticuloPrecioEntity>
{
    public void Configure(EntityTypeBuilder<ArticuloPrecioEntity> entity)
    {
        entity.ToTable("ARTICULO_PRECIO");

        entity.HasKey(p => p.IdArticuloPrecio);

        entity.Property(p => p.IdArticuloPrecio)
            .HasColumnName("ID_ARTICULO_PRECIO");

        entity.Property(p => p.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50);

        entity.Property(p => p.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);

        entity.Property(p => p.Precio1)
            .HasColumnName("PRECIO1");

        entity.Property(p => p.Precio2)
            .HasColumnName("PRECIO2");

        entity.Property(p => p.Precio3)
            .HasColumnName("PRECIO3");

        entity.Property(p => p.Relleno)
            .HasColumnName("RELLENO");
    }
}

