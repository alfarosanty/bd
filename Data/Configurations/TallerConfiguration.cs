using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TallerConfiguration : IEntityTypeConfiguration<Taller>
{
    public void Configure(EntityTypeBuilder<Taller> builder)
    {
        builder.ToTable(Taller.TABLA);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_FABRICANTE");

        builder.Property(x => x.razonSocial)
               .HasColumnName("RAZON_SOCIAL")
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(x => x.telefono)
               .HasColumnName("TELEFONO")
               .HasMaxLength(50);

        builder.Property(x => x.direccion)
               .HasColumnName("DIRECCION")
               .HasMaxLength(200);

        builder.Property(x => x.provincia)
               .HasColumnName("PROVINCIA")
               .HasMaxLength(100);
    }
}