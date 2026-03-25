using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlumeAPI.Data.Configurations
{
public class ArticuloPrecioConfiguration 
    : IEntityTypeConfiguration<ArticuloPrecio>
{
    public void Configure(EntityTypeBuilder<ArticuloPrecio> builder)
    {
        builder.ToTable("ARTICULO_PRECIO");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("ID_ARTICULO_PRECIO");

        builder.Property(p => p.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50);

        builder.Property(p => p.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);

        builder.Property(p => p.Precio1)
            .HasColumnName("PRECIO1");

        builder.Property(p => p.Precio2)
            .HasColumnName("PRECIO2");

        builder.Property(p => p.Precio3)
            .HasColumnName("PRECIO3");

        builder.Property(p => p.Relleno)
            .HasColumnName("RELLENO");
    }
}
    
}

