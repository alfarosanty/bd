using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DatosAfipConfiguration : IEntityTypeConfiguration<DatosAfip>
{
    public void Configure(EntityTypeBuilder<DatosAfip> builder)
    {
        builder.ToTable("DATOS_AFIP");
        builder.HasKey(x => x.Servicio);
        
        builder.Property(x => x.Servicio).HasColumnName("SERVICIO");
        builder.Property(x => x.Certificado).HasColumnName("CERTIFICADO");
        builder.Property(x => x.Contrasena).HasColumnName("CONTRASENA");
        builder.Property(x => x.UrlWsaa).HasColumnName("URLWSAA");
    }
}