using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using MediatR;

namespace CatalogoItau.Application.Queries.Pedidos;

public sealed class GetPedidosQuery : IRequest<IEnumerable<Pedido>>
{
}
public sealed class GetPedidosHandler : IRequestHandler<GetPedidosQuery, IEnumerable<Pedido>>
{
    private readonly IPedidoRepository _repository;

    public GetPedidosHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Pedido>> Handle(GetPedidosQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ObterTodosAsync();
    }
}