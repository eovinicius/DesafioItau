namespace CatalogoItau.Infrastructure.Percistence.Mapping;

using CatalogoItau.Domain.Entities;

using Microsoft.EntityFrameworkCore;

public static class PedidoMapping
{
    public static void ConfigurePedido(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.NumeroPedido)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(p => p.NumeroPedido)
                .IsUnique();

            entity.Property(p => p.ClienteNome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.ClienteEmail)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Status)
                .HasConversion<string>();

            entity.HasMany(p => p.Itens)
                .WithOne()
                .HasForeignKey(i => i.PedidoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
