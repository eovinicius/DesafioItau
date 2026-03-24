using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace CatalogoItau.Infrastructure.Percistence.Repositories;

public sealed class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task<Pedido?> ObterPorIdAsync(int id)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pedido>> ObterTodosAsync()
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Pedido> items, int totalCount)> ObterTodosComPaginacaoAsync(int page, int pageSize)
    {
        var totalCount = await _context.Pedidos.CountAsync();
        var items = await _context.Pedidos
            .Include(p => p.Itens)
            .OrderByDescending(p => p.DataPedido)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<Pedido?> ObterPorNumeroAsync(string numeroPedido)
    {
        return await _context.Pedidos
            .FirstOrDefaultAsync(p => p.NumeroPedido == numeroPedido);
    }
}