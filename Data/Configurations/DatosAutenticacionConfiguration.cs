using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DatosAutenticacionConfiguration : IEntityTypeConfiguration<DatosAutenticacion>
{
    public void Configure(EntityTypeBuilder<DatosAutenticacion> builder)
    {
        builder.ToTable("DATOS_AUTENTICACION");
        builder.HasKey(x => x.UniqueId); // Asumiendo que UniqueId es la PK
        
        builder.Property(x => x.UniqueId).HasColumnName("UNIQUE_ID");
        builder.Property(x => x.Token).HasColumnName("TOKEN");
        builder.Property(x => x.Firma).HasColumnName("FIRMA");
        builder.Property(x => x.Expiracion).HasColumnName("EXPIRACION");
    }
}