using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DatosAutenticacionConfiguration : IEntityTypeConfiguration<DatosAutenticacion>
{
    public void Configure(EntityTypeBuilder<DatosAutenticacion> builder)
    {
        builder.ToTable("DATOS_AUTENTICACION");

        builder.Property(x => x.Id)
       .HasColumnName("ID")
       .ValueGeneratedOnAdd()
       .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(x => x.UniqueId).HasColumnName("UNIQUE_ID").IsRequired();
        builder.Property(x => x.Token).HasColumnName("TOKEN").IsRequired();
        builder.Property(x => x.Firma).HasColumnName("FIRMA").IsRequired();
        builder.Property(x => x.Expiracion).HasColumnName("EXPIRACION").IsRequired();
        
        builder.Property(x => x.Servicio).HasColumnName("SERVICIO").IsRequired().HasMaxLength(50);
        
        builder.HasIndex(x => x.Servicio).IsUnique(); 
    }
}