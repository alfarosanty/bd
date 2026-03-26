using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Entities.clases.modelo;

public class MedidaConfiguration : IEntityTypeConfiguration<Medida>
{
    public void Configure(EntityTypeBuilder<Medida> builder)
    {
        builder.ToTable("MEDIDA");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("ID_MEDIDA");

        builder.Property(m => m.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50);

        builder.Property(m => m.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);
    }
}
