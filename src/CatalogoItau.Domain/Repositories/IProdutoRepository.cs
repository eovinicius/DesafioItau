using CatalogoItau.Domain.Entities;

namespace CatalogoItau.Domain.Repositories;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(int id);

    Task<Produto?> ObterPorIdParaAtualizacaoAsync(int id);

    Task<IEnumerable<Produto>> ObterAtivosAsync();

    Task<(IEnumerable<Produto> items, int totalCount)> ObterAtivosComPaginacaoAsync(int page, int pageSize);

    Task AdicionarAsync(Produto produto);

    Task AtualizarAsync(Produto produto);
}