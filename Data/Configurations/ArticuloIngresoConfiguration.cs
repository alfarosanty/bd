using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ArticuloIngresoConfiguration : IEntityTypeConfiguration<ArticuloIngreso>
{
    public void Configure(EntityTypeBuilder<ArticuloIngreso> builder)
    {
        builder.ToTable(ArticuloIngreso.TABLA);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("ID_ARTICULO_INGRESO")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.IdIngreso)
            .HasColumnName("ID_INGRESO");

        builder.Property(p => p.IdArticulo)
            .HasColumnName("ID_ARTICULO")
            .IsRequired();

        builder.Property(p => p.Cantidad)
            .HasColumnName("CANTIDAD")
            .IsRequired();

        builder.Property(p => p.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50);

        builder.Property(p => p.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);

        builder.Property(p => p.Fecha)
            .HasColumnName("FECHA");

        builder.HasOne(p => p.Articulo)
            .WithMany()
            .HasForeignKey(p => p.IdArticulo)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Ingreso)
            .WithMany(p => p.Articulos)
            .HasForeignKey(p => p.IdIngreso)
            .OnDelete(DeleteBehavior.Cascade);
    }
}