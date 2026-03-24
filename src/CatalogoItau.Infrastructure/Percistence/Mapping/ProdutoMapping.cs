namespace CatalogoItau.Infrastructure.Percistence.Mapping;

using CatalogoItau.Domain.Entities;

using Microsoft.EntityFrameworkCore;

public static class ProdutoMapping
{
    public static void ConfigureProduto(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Descricao)
                .HasMaxLength(500);

            entity.Property(p => p.Preco)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(p => p.Ativo)
                .HasDefaultValue(true);

            entity.Property(p => p.AtualizadoEm)
                .IsRequired()
                .IsConcurrencyToken();
        });
    }
}
