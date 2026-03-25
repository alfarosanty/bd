namespace BlumeAPI.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

public class CondicionFiscalConfiguration : IEntityTypeConfiguration<CondicionFiscal>
{
    public void Configure(EntityTypeBuilder<CondicionFiscal> builder)
    {
        builder.ToTable("CONDICION_AFIP");

        builder.HasKey(c => c.IdCondicion);

        builder.Property(c => c.IdCondicion)
            .HasColumnName("ID_CONDICION");

        builder.Property(c => c.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200)
            .IsRequired();
    }
}