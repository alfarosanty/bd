using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlumeAPI.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("CLIENTE");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("ID_CLIENTE");

            builder.Property(c => c.RazonSocial)
                .HasColumnName("RAZON_SOCIAL")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.Telefono)
                .HasColumnName("TELEFONO")
                .HasMaxLength(50);

            builder.Property(c => c.Contacto)
                .HasColumnName("CONTACTO")
                .HasMaxLength(100);

            builder.Property(c => c.Domicilio)
                .HasColumnName("DOMICILIO")
                .HasMaxLength(200);

            builder.Property(c => c.Localidad)
                .HasColumnName("LOCALIDAD")
                .HasMaxLength(100);

            builder.Property(c => c.Cuit)
                .HasColumnName("CUIT")
                .HasMaxLength(20);

            builder.Property(c => c.IdCondicionFiscal)
                .HasColumnName("ID_CONDICION_AFIP");

            builder.Property(c => c.Provincia)
                .HasColumnName("PROVINCIA")
                .HasMaxLength(100);

            builder.Property(c => c.Transporte)
                .HasColumnName("TRANSPORTE")
                .HasMaxLength(100);

            builder.Property(c => c.Validado)
                .HasColumnName("VALIDO");

            builder.HasOne(x => x.CondicionFiscal)
               .WithMany()
               .HasForeignKey(x => x.IdCondicionFiscal);
        }
    }
}