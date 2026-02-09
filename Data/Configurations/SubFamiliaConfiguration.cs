using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

public class SubFamiliaConfiguration : IEntityTypeConfiguration<SubFamiliaEntity>
{
    public void Configure(EntityTypeBuilder<SubFamiliaEntity> entity)
    {
        entity.ToTable("SUBFAMILIA");

        entity.HasKey(s => s.IdSubFamilia);

        entity.Property(s => s.IdSubFamilia)
            .HasColumnName("ID_SUBFAMILIA");

        entity.Property(s => s.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50);

        entity.Property(s => s.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);
    }
}
