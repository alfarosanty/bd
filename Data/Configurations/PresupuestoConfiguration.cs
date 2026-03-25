using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PresupuestoConfiguration : IEntityTypeConfiguration<Presupuesto>
{
    public void Configure(EntityTypeBuilder<Presupuesto> builder)
    {
        builder.ToTable("PRESUPUESTO");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_PRESUPUESTO");

        builder.Property(x => x.Fecha)
               .HasColumnName("FECHA_PRESUPUESTO")
               .HasColumnType("date");

        builder.Property(x => x.FechaCreacion)
               .HasColumnName("FECHA_CREACION")
               .HasColumnType("date");

        builder.Property(x => x.IdCliente).HasColumnName("ID_CLIENTE");
        builder.Property(x => x.IdEstado).HasColumnName("ID_ESTADO");
        builder.Property(x => x.IdFactura).HasColumnName("ID_FACTURA");
        builder.Property(x => x.EximirIVA).HasColumnName("EXMIR_IVA");
        builder.Property(x => x.Total).HasColumnName("TOTAL_PRESUPUESTO").HasColumnType("numeric(18,2)");
        builder.Property(x => x.DescuentoGeneral).HasColumnName("DESCUENTO").HasColumnType("numeric(18,2)");

        builder.HasOne(x => x.Cliente)
               .WithMany()
               .HasForeignKey(x => x.IdCliente);

        builder.HasOne(x => x.EstadoPresupuesto)
               .WithMany()
               .HasForeignKey(x => x.IdEstado);

        builder.HasMany(x => x.Articulos)
               .WithOne(x => x.Presupuesto)
               .HasForeignKey(x => x.IdPresupuesto);
    }
}