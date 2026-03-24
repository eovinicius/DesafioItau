using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using MediatR;

namespace CatalogoItau.Application.Queries.Pedidos;

public sealed class GetPedidoByIdQuery : IRequest<Pedido?>
{
    public int Id { get; set; }
}

public sealed class GetPedidoByIdHandler : IRequestHandler<GetPedidoByIdQuery, Pedido?>
{
    private readonly IPedidoRepository _repository;

    public GetPedidoByIdHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Pedido?> Handle(GetPedidoByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ObterPorIdAsync(request.Id);
    }
}