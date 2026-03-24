using CatalogoItau.Application.Abstractions.Pagination;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using MediatR;

namespace CatalogoItau.Application.Queries.Produtos;

public sealed class GetProdutosPaginadosQuery : IRequest<PagedResult<Produto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetProdutosPaginadosQuery(int page, int pageSize)
    {
        Page = page > 0 ? page : 1;
        PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10;
    }
}

public sealed class GetProdutosPaginadosHandler : IRequestHandler<GetProdutosPaginadosQuery, PagedResult<Produto>>
{
    private readonly IProdutoRepository _repository;

    public GetProdutosPaginadosHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Produto>> Handle(GetProdutosPaginadosQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.ObterAtivosComPaginacaoAsync(request.Page, request.PageSize);
        return new PagedResult<Produto>(items, request.Page, request.PageSize, totalCount);
    }
}
