using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

public class ColorConfiguration : IEntityTypeConfiguration<ColorEntity>
{
    public void Configure(EntityTypeBuilder<ColorEntity> entity)
    {
        entity.ToTable("COLOR");

        entity.HasKey(c => c.IdColor);

        entity.Property(c => c.IdColor)
            .HasColumnName("ID_COLOR");

        entity.Property(c => c.Codigo)
            .HasColumnName("CODIGO");

        entity.Property(c => c.Descripcion)
            .HasColumnName("DESCRIPCION");

        entity.Property(c => c.ColorHexa)
            .HasColumnName("HEXA");
    }
}

