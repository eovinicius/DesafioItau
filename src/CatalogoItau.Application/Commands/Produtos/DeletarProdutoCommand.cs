using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Repositories;

using MediatR;

namespace CatalogoItau.Application.Commands.Produtos;

public sealed class DeletarProdutoCommand : IRequest
{
    public int Id { get; set; }
}

public sealed class DeletarProdutoHandler : IRequestHandler<DeletarProdutoCommand>
{
    private readonly IProdutoRepository _repository;

    public DeletarProdutoHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeletarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _repository.ObterPorIdAsync(request.Id);

        if (produto == null || !produto.Ativo)
            throw new NotFoundException("Produto não encontrado");

        produto.Inativar();

        await _repository.AtualizarAsync(produto);
    }
}