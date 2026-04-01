using BlumeAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class IngresoConfiguration : IEntityTypeConfiguration<Ingreso>
{
    public void Configure(EntityTypeBuilder<Ingreso> builder)
    {
        builder.ToTable("INGRESO");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("ID_INGRESO")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Fecha)
            .HasColumnName("FECHA")
            .IsRequired();

        builder.Property(p => p.IdTaller)
            .HasColumnName("ID_TALLER")
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasColumnName("ESTADO_INGRESO")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v.ToString().ToUpper(), 
                v => (EstadoIngreso)Enum.Parse(typeof(EstadoIngreso), v, true)
                );

        builder.HasOne(p => p.Taller)
            .WithMany()
            .HasForeignKey(p => p.IdTaller)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Articulos)
            .WithOne()
            .HasForeignKey("ID_INGRESO") 
            .OnDelete(DeleteBehavior.Cascade);
    }
}