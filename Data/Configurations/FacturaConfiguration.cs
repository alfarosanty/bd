using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

namespace BlumeAPI.Data.Configurations
{
    public class FacturaConfiguration : IEntityTypeConfiguration<FacturaEntity>
    {
        public void Configure(EntityTypeBuilder<FacturaEntity> entity)
        {
            entity.ToTable("FACTURA");

            entity.HasKey(f => f.IdFactura);

            entity.Property(f => f.IdFactura)
                .HasColumnName("ID_FACTURA");

            entity.Property(f => f.IdCliente)
                .HasColumnName("ID_CLIENTE");

            entity.Property(f => f.FechaFactura)
                .HasColumnName("FECHA_FACTURA");

            entity.Property(f => f.ImporteBruto)
                .HasColumnName("IMPORTE_BRUTO");

            entity.Property(f => f.EximirIva)
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

            entity.Property(f => f.Descuento)
                .HasColumnName("DESCUENTO");

            entity.Property(f => f.IdPresupuesto)
                .HasColumnName("ID_PRESUPUESTO");

            // 🔗 Relación con Cliente
            entity.HasOne(f => f.Cliente)
                .WithMany()
                .HasForeignKey(f => f.IdCliente);
        }
    }
}