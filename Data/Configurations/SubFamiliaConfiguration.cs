using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Entities;

public class SubFamiliaConfiguration : IEntityTypeConfiguration<SubFamilia>
{
    public void Configure(EntityTypeBuilder<SubFamilia> builder)
    {
        builder.ToTable("SUBFAMILIA");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("ID_SUBFAMILIA");

        builder.Property(s => s.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50);

        builder.Property(s => s.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);
    }
}
