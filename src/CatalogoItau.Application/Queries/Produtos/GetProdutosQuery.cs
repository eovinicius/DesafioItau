using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using MediatR;

namespace CatalogoItau.Application.Queries.Produtos;

public sealed class GetProdutosQuery : IRequest<IEnumerable<Produto>> { }

public sealed class GetProdutosHandler : IRequestHandler<GetProdutosQuery, IEnumerable<Produto>>
{
    private readonly IProdutoRepository _repository;

    public GetProdutosHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Produto>> Handle(GetProdutosQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ObterAtivosAsync();
    }
}