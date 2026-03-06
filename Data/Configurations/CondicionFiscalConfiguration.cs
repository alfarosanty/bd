namespace BlumeAPI.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

public class CondicionFiscalConfiguration : IEntityTypeConfiguration<CondicionFiscalEntity>
{
    public void Configure(EntityTypeBuilder<CondicionFiscalEntity> entity)
    {
        entity.ToTable("CONDICION_AFIP");

        entity.HasKey(c => c.IdCondicion);

        entity.Property(c => c.IdCondicion)
            .HasColumnName("ID_CONDICION");

        entity.Property(c => c.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(c => c.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200)
            .IsRequired();
    }
}