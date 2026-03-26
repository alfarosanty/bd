using BlumeAPI.Entities.clases.modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlumeAPI.Data.Configurations
{
    
public class NotaDeCreditoConfiguration : IEntityTypeConfiguration<NotaDeCredito>
{
    public void Configure(EntityTypeBuilder<NotaDeCredito> builder)
    {
        builder.ToTable("NOTA_CREDITO");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_NOTA_CREDITO");

        builder.Property(x => x.FechaNota)
       .HasColumnName("FECHA_NOTA_CREDITO")
       .HasColumnType("date");
        builder.Property(x => x.IdCliente).HasColumnName("ID_CLIENTE");
        builder.Property(x => x.ImporteBruto).HasColumnName("IMPORTE_BRUTO").HasColumnType("numeric(18,2)");
        builder.Property(x => x.ImporteNeto).HasColumnName("IMPORTE_NETO").HasColumnType("numeric(18,2)");
        builder.Property(x => x.Iva).HasColumnName("IVA").HasColumnType("numeric(18,2)");
        builder.Property(x => x.Descuento).HasColumnName("DESCUENTO").HasColumnType("numeric(18,2)");
        builder.Property(x => x.TipoNota).HasColumnName("TIPO_NOTA_CREDITO");
        builder.Property(x => x.PuntoDeVenta).HasColumnName("PUNTO_DE_VENTA");
        builder.Property(x => x.Cae).HasColumnName("CAE_NUMERO");
        builder.Property(x => x.FechaVencimientoCae)
       .HasColumnName("FECHA_VENCIMIENTO_CAE")
       .HasColumnType("date");
               builder.Property(x => x.NumeroComprobante).HasColumnName("NUMERO_NOTA_CREDITO");
        builder.Property(x => x.IdFacturaAsociada).HasColumnName("ID_FACTURA");
        builder.Property(x => x.Motivo).HasColumnName("MOTIVO");

        // Navegación
        builder.HasOne(x => x.Cliente)
               .WithMany()
               .HasForeignKey(x => x.IdCliente);

        builder.HasMany(x => x.Articulos)
               .WithOne()
               .HasForeignKey(x => x.IdNotaDeCredito);

        // Ignorar propiedades de navegación pura sin columna
        builder.Ignore(x => x.Articulos); // EF lo maneja por HasMany
    }
}
}