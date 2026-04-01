using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlumeAPI.Data.Configurations
{
    
public class ArticuloNotaCreditoConfiguration : IEntityTypeConfiguration<ArticuloNotaCredito>
{
    public void Configure(EntityTypeBuilder<ArticuloNotaCredito> builder)
    {
        builder.ToTable("ARTICULO_NOTA_CREDITO");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ID_ARTICULO_NOTA_CREDITO");

        builder.Property(x => x.IdNotaDeCredito).HasColumnName("ID_NOTA_CREDITO");
        builder.Property(x => x.IdArticulo).HasColumnName("ID_ARTICULO");
        builder.Property(x => x.Codigo).HasColumnName("CODIGO");
        builder.Property(x => x.Descripcion).HasColumnName("DESCRIPCION");
        builder.Property(x => x.Cantidad).HasColumnName("CANTIDAD");
        builder.Property(x => x.Descuento).HasColumnName("DESCUENTO").HasColumnType("numeric(18,2)");

        // Calculados — no se persisten
        builder.Ignore(x => x.PrecioUnitario);
        builder.Ignore(x => x.MontoBruto);
        builder.Ignore(x => x.MontoNeto);
        builder.Ignore(x => x.Iva);

        // Navegación
        builder.HasOne(x => x.Articulo)
               .WithMany()
               .HasForeignKey(x => x.IdArticulo);
    }
}
}