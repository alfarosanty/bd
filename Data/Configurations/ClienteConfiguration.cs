using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

namespace BlumeAPI.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<ClienteEntity>
    {
        public void Configure(EntityTypeBuilder<ClienteEntity> entity)
        {
            entity.ToTable("CLIENTE");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .HasColumnName("ID_CLIENTE");

            entity.Property(c => c.RazonSocial)
                .HasColumnName("RAZON_SOCIAL")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(c => c.Telefono)
                .HasColumnName("TELEFONO")
                .HasMaxLength(50);

            entity.Property(c => c.Contacto)
                .HasColumnName("CONTACTO")
                .HasMaxLength(100);

            entity.Property(c => c.Domicilio)
                .HasColumnName("DOMICILIO")
                .HasMaxLength(200);

            entity.Property(c => c.Localidad)
                .HasColumnName("LOCALIDAD")
                .HasMaxLength(100);

            entity.Property(c => c.Cuit)
                .HasColumnName("CUIT")
                .HasMaxLength(20);

            entity.Property(c => c.IdCondicionFiscal)
                .HasColumnName("ID_CONDICION_AFIP");

            entity.Property(c => c.Provincia)
                .HasColumnName("PROVINCIA")
                .HasMaxLength(100);

            entity.Property(c => c.Transporte)
                .HasColumnName("TRANSPORTE")
                .HasMaxLength(100);

            entity.Property(c => c.Valido)
                .HasColumnName("VALIDO");
        }
    }
}