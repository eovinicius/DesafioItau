using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Repositories;

using FluentValidation;

using MediatR;

namespace CatalogoItau.Application.Commands.Produtos;

public sealed class AtualizarProdutoCommand : IRequest
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
}


public sealed class AtualizarProdutoHandler : IRequestHandler<AtualizarProdutoCommand>
{
    private readonly IProdutoRepository _repository;

    public AtualizarProdutoHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AtualizarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _repository.ObterPorIdAsync(request.Id);

        if (produto == null || !produto.Ativo)
            throw new NotFoundException("Produto não encontrado");

        produto.Atualizar(
            request.Nome,
            request.Preco,
            request.Estoque,
            request.Descricao
        );

        await _repository.AtualizarAsync(produto);
    }
}


public sealed class AtualizarProdutoValidator : AbstractValidator<AtualizarProdutoCommand>
{
    public AtualizarProdutoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

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