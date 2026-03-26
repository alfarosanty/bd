using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlumeAPI.Data.Configurations
{
    public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
    {
        public void Configure(EntityTypeBuilder<Factura> entity)
        {
            entity.ToTable("FACTURA");

            entity.HasKey(f => f.Id);

            entity.Property(f => f.Id)
                .HasColumnName("ID_FACTURA");

            entity.Property(f => f.IdCliente)
                .HasColumnName("ID_CLIENTE");

            entity.Property(f => f.FechaFactura)
                .HasColumnName("FECHA_FACTURA");

            entity.Property(f => f.ImporteBruto)
                .HasColumnName("IMPORTE_BRUTO");

            entity.Property(f => f.EximirIVA)
                .HasColumnName("EXIMIR_IVA");

            entity.Property(f => f.PuntoDeVenta)
                .HasColumnName("PUNTO_DE_VENTA");

            entity.Property(f => f.NumeroComprobante)
                .HasColumnName("NUMERO_FACTURA");

            entity.Property(f => f.CaeNumero)
                .HasColumnName("CAE_NUMERO")
                .HasMaxLength(50);

            entity.Property(f => f.FechaVencimientoCae)
                .HasColumnName("FECHA_VENCIMIENTO_CAE");

            entity.Property(f => f.ImporteNeto)
                .HasColumnName("IMPORTE_NETO");

            entity.Property(f => f.Iva)
                .HasColumnName("IVA");

            entity.Property(f => f.TipoFactura)
                .HasColumnName("TIPO_FACTURA")
                .HasMaxLength(2);

            entity.Property(f => f.DescuentoGeneral)
                .HasColumnName("DESCUENTO");

            entity.Property(f => f.IdPresupuesto)
                .HasColumnName("ID_PRESUPUESTO")
                .IsRequired(false);

            // 🔗 Relaciones con entidades
            entity.HasOne(f => f.Cliente)
                .WithMany()
                .HasForeignKey(f => f.IdCliente);
            
            entity.HasOne(f => f.Presupuesto)
                .WithMany()
                .HasForeignKey(f => f.IdPresupuesto);
        }
    }
}