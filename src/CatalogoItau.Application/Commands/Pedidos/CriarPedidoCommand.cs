using CatalogoItau.Application.Abstractions.Data;
using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Exceptions;
using CatalogoItau.Domain.Repositories;

using FluentValidation;

using MediatR;

namespace CatalogoItau.Application.Commands.Pedidos;

public sealed class CriarPedidoCommand : IRequest<Pedido>
{
    public string NumeroPedido { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public List<ItemDto> Itens { get; set; }
    public sealed class ItemDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }

    public sealed class CriarPedidoHandler : IRequestHandler<CriarPedidoCommand, Pedido>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarPedidoHandler(
            IPedidoRepository pedidoRepository,
            IProdutoRepository produtoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Pedido> Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
        {
            var existingPedido = await _pedidoRepository.ObterPorNumeroAsync(request.NumeroPedido);
            if (existingPedido != null) throw new DomainException("Número de pedido já existe");

            try
            {
                _unitOfWork.BeginTransaction();

                var pedido = new Pedido(
                    request.NumeroPedido,
                    request.ClienteNome,
                    request.ClienteEmail
                );

                foreach (var item in request.Itens)
                {
                    var produto = await _produtoRepository.ObterPorIdParaAtualizacaoAsync(item.ProdutoId);

                    if (produto == null || !produto.Ativo)
                        throw new NotFoundException($"Produto {item.ProdutoId} não encontrado");

                    produto.BaixarEstoque(item.Quantidade);
                    pedido.AdicionarItem(produto, item.Quantidade);
                }

                await _pedidoRepository.AdicionarAsync(pedido);

                _unitOfWork.Commit();

                return pedido;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}


public sealed class CriarPedidoValidator : AbstractValidator<CriarPedidoCommand>
{
    public CriarPedidoValidator()
    {
        RuleFor(x => x.NumeroPedido)
            .NotEmpty();

        RuleFor(x => x.ClienteNome)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ClienteEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Itens)
            .NotEmpty();

        RuleForEach(x => x.Itens)
            .SetValidator(new ItemPedidoValidator());
    }
}

public sealed class ItemPedidoValidator : AbstractValidator<CriarPedidoCommand.ItemDto>
{
    public ItemPedidoValidator()
    {
        RuleFor(x => x.ProdutoId)
            .GreaterThan(0)
            .WithMessage("ProdutoId deve ser maior que zero");

        RuleFor(x => x.Quantidade)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Quantidade deve ser maior ou igual a 1");
    }
}