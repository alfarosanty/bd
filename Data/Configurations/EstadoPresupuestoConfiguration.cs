using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EstadoPresupuestoConfiguration : IEntityTypeConfiguration<EstadoPresupuesto>
{
    public void Configure(EntityTypeBuilder<EstadoPresupuesto> builder)
    {
        builder.ToTable("ESTADO_PRESUPUESTO");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_ESTADO");
        builder.Property(x => x.Codigo).HasColumnName("CODIGO");
        builder.Property(x => x.Descripcion).HasColumnName("DESCRIPCION");
    }
}