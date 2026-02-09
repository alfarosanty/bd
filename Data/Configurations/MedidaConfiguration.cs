using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

public class MedidaConfiguration : IEntityTypeConfiguration<MedidaEntity>
{
    public void Configure(EntityTypeBuilder<MedidaEntity> entity)
    {
        entity.ToTable("MEDIDA");

        entity.HasKey(m => m.IdMedida);

        entity.Property(m => m.IdMedida)
            .HasColumnName("ID_MEDIDA");

        entity.Property(m => m.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50);

        entity.Property(m => m.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);
    }
}
