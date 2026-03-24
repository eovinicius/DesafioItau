using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace CatalogoItau.Infrastructure.Percistence.Repositories;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(int id)
    {
        return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto?> ObterPorIdParaAtualizacaoAsync(int id)
    {
        return await _context.Produtos
            .FromSqlInterpolated($"SELECT * FROM \"Produtos\" WHERE \"Id\" = {id} FOR UPDATE")
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Produto>> ObterAtivosAsync()
    {
        return await _context.Produtos
            .Where(p => p.Ativo)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Produto> items, int totalCount)> ObterAtivosComPaginacaoAsync(int page, int pageSize)
    {
        var totalCount = await _context.Produtos.CountAsync(p => p.Ativo);
        var items = await _context.Produtos
            .Where(p => p.Ativo)
            .OrderByDescending(p => p.DataCriacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }
}