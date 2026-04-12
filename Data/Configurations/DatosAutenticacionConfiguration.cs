using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DatosAutenticacionConfiguration : IEntityTypeConfiguration<DatosAutenticacion>
{
    public void Configure(EntityTypeBuilder<DatosAutenticacion> builder)
    {
        builder.ToTable("DATOS_AUTENTICACION");

        // 1. Definimos el Id como Clave Primaria
        builder.Property(x => x.Id)
       .HasColumnName("ID")
       .ValueGeneratedOnAdd()
       .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        // 2. Mapeamos el resto de las propiedades
        builder.Property(x => x.UniqueId).HasColumnName("UNIQUE_ID").IsRequired();
        builder.Property(x => x.Token).HasColumnName("TOKEN").IsRequired();
        builder.Property(x => x.Firma).HasColumnName("FIRMA").IsRequired();
        builder.Property(x => x.Expiracion).HasColumnName("EXPIRACION").IsRequired();
        
        // 3. ¡IMPORTANTE! Agregamos la columna Servicio
        builder.Property(x => x.Servicio).HasColumnName("SERVICIO").IsRequired().HasMaxLength(50);
        
        // 4. (Recomendado) Índice único por servicio para mayor performance y seguridad
        builder.HasIndex(x => x.Servicio).IsUnique(); 
    }
}