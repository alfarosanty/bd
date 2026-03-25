using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

public class ArticuloConfiguration : IEntityTypeConfiguration<Articulo>
{
    public void Configure(EntityTypeBuilder<Articulo> builder)
    {
        builder.ToTable("ARTICULO");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("ID_ARTICULO");

        builder.Property(a => a.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);

        builder.Property(a => a.IdColor)
            .HasColumnName("ID_COLOR");

        builder.Property(a => a.IdMedida)
            .HasColumnName("ID_MEDIDA");

        builder.Property(a => a.IdSubFamilia)
            .HasColumnName("ID_SUBFAMILIA");

        builder.Property(a => a.IdArticuloPrecio)
            .HasColumnName("ID_ARTICULO_PRECIO");

        builder.Property(a => a.IdFabricante)
            .HasColumnName("ID_FABRICANTE");

        builder.Property(a => a.Habilitado)
            .HasColumnName("HABILITADO");

        builder.Property(a => a.Stock)
            .HasColumnName("STOCK");

        builder.Property(a => a.IdAsociadoRelleno)
            .HasColumnName("ID_ASOCIADO_RELLENO");

        // 🔗 Relaciones
        builder.HasOne(a => a.Color)
            .WithMany()
            .HasForeignKey(a => a.IdColor);

        builder.HasOne(a => a.Medida)
            .WithMany()
            .HasForeignKey(a => a.IdMedida);

        builder.HasOne(a => a.SubFamilia)
            .WithMany()
            .HasForeignKey(a => a.IdSubFamilia);

        builder.HasOne(a => a.ArticuloPrecio)
            .WithMany()
            .HasForeignKey(a => a.IdArticuloPrecio);


        builder.Ignore(a => a.Nuevo);
        builder.Ignore(a => a.CantidadEnCorte);
        builder.Ignore(a => a.CantidadEnTaller);
    }
}
