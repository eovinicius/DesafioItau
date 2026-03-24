using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using MediatR;

namespace CatalogoItau.Application.Queries.Produtos;

public sealed class GetProdutoByIdQuery : IRequest<Produto?>
{
    public int Id { get; set; }
}


public sealed class GetProdutoByIdHandler : IRequestHandler<GetProdutoByIdQuery, Produto?>
{
    private readonly IProdutoRepository _repository;

    public GetProdutoByIdHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Produto?> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ObterPorIdAsync(request.Id);
    }
}