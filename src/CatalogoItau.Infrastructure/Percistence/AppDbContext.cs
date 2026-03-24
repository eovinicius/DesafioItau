namespace CatalogoItau.Infrastructure.Percistence;

using CatalogoItau.Domain.Entities;
using CatalogoItau.Infrastructure.Percistence.Mapping;

using Microsoft.EntityFrameworkCore;


public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureProduto();
        modelBuilder.ConfigurePedido();
        modelBuilder.ConfigureItemPedido();
    }
}