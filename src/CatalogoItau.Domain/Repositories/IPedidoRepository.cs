using CatalogoItau.Domain.Entities;

namespace CatalogoItau.Domain.Repositories;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(int id);

    Task<IEnumerable<Pedido>> ObterTodosAsync();

    Task<(IEnumerable<Pedido> items, int totalCount)> ObterTodosComPaginacaoAsync(int page, int pageSize);

    Task<Pedido?> ObterPorNumeroAsync(string numeroPedido);

    Task AdicionarAsync(Pedido pedido);

    Task AtualizarAsync(Pedido pedido);
}