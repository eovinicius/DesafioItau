using CatalogoItau.Application.Abstractions.Pagination;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using MediatR;

namespace CatalogoItau.Application.Queries.Pedidos;

public sealed class GetPedidosPaginadosQuery : IRequest<PagedResult<Pedido>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetPedidosPaginadosQuery(int page, int pageSize)
    {
        Page = page > 0 ? page : 1;
        PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10;
    }
}

public sealed class GetPedidosPaginadosHandler : IRequestHandler<GetPedidosPaginadosQuery, PagedResult<Pedido>>
{
    private readonly IPedidoRepository _repository;

    public GetPedidosPaginadosHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Pedido>> Handle(GetPedidosPaginadosQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.ObterTodosComPaginacaoAsync(request.Page, request.PageSize);
        return new PagedResult<Pedido>(items, request.Page, request.PageSize, totalCount);
    }
}
