using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlumeAPI.Data.Entities;

public class ArticuloConfiguration : IEntityTypeConfiguration<ArticuloEntity>
{
    public void Configure(EntityTypeBuilder<ArticuloEntity> entity)
    {
        entity.ToTable("ARTICULO");

        entity.HasKey(a => a.IdArticulo);

        entity.Property(a => a.IdArticulo)
            .HasColumnName("ID_ARTICULO");

        entity.Property(a => a.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(a => a.Descripcion)
            .HasColumnName("DESCRIPCION")
            .HasMaxLength(200);

        entity.Property(a => a.IdColor)
            .HasColumnName("ID_COLOR");

        entity.Property(a => a.IdMedida)
            .HasColumnName("ID_MEDIDA");

        entity.Property(a => a.IdSubFamilia)
            .HasColumnName("ID_SUBFAMILIA");

        entity.Property(a => a.IdArticuloPrecio)
            .HasColumnName("ID_ARTICULO_PRECIO");

        entity.Property(a => a.IdFabricante)
            .HasColumnName("ID_FABRICANTE");

        entity.Property(a => a.Habilitado)
            .HasColumnName("HABILITADO");

        entity.Property(a => a.Stock)
            .HasColumnName("STOCK");

        entity.Property(a => a.IdAsociadoRelleno)
            .HasColumnName("ID_ASOCIADO_RELLENO");

        // 🔗 Relaciones
        entity.HasOne(a => a.Color)
            .WithMany()
            .HasForeignKey(a => a.IdColor);

        entity.HasOne(a => a.Medida)
            .WithMany()
            .HasForeignKey(a => a.IdMedida);

        entity.HasOne(a => a.SubFamilia)
            .WithMany()
            .HasForeignKey(a => a.IdSubFamilia);

        entity.HasOne(a => a.ArticuloPrecio)
            .WithMany()
            .HasForeignKey(a => a.IdArticuloPrecio);
    }
}
