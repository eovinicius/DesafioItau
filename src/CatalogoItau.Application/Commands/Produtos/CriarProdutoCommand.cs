using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using FluentValidation;

using MediatR;

namespace CatalogoItau.Application.Commands.Produtos;

public sealed record CriarProdutoCommand(
    string Nome,
    string? Descricao,
    decimal Preco,
    int Estoque
) : IRequest<int>;

public sealed class CriarProdutoHandler : IRequestHandler<CriarProdutoCommand, int>
{
    private readonly IProdutoRepository _repository;

    public CriarProdutoHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CriarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = new Produto(
            request.Nome,
            request.Preco,
            request.Estoque,
            request.Descricao
        );

        await _repository.AdicionarAsync(produto);

        return produto.Id;
    }
}

public sealed class CriarProdutoValidator : AbstractValidator<CriarProdutoCommand>
{
    public CriarProdutoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Descricao)
            .MaximumLength(500);

        RuleFor(x => x.Preco)
            .GreaterThan(0);

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0);
    }
}