namespace CatalogoItau.Infrastructure.Percistence.Mapping;

using CatalogoItau.Domain.Entities;

using Microsoft.EntityFrameworkCore;

public static class ItemPedidoMapping
{
    public static void ConfigureItemPedido(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemPedido>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Quantidade)
                .IsRequired();

            entity.Property(i => i.PrecoUnitario)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.HasOne<Produto>()
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
