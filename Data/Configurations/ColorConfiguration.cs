using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        builder.ToTable("COLOR");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("ID_COLOR");

        builder.Property(c => c.Codigo)
            .HasColumnName("CODIGO");

        builder.Property(c => c.Descripcion)
            .HasColumnName("DESCRIPCION");

        builder.Property(c => c.ColorHexa)
            .HasColumnName("HEXA");
    }
}

