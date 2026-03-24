using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Entities.Enums;
using CatalogoItau.Domain.Repositories;

using FluentValidation;

using MediatR;

namespace CatalogoItau.Application.Commands.Pedidos;

public sealed class AtualizarStatusPedidoCommand : IRequest
{
    public int Id { get; set; }
    public StatusPedido NovoStatus { get; set; }
}


public sealed class AtualizarStatusPedidoHandler : IRequestHandler<AtualizarStatusPedidoCommand>
{
    private readonly IPedidoRepository _repository;

    public AtualizarStatusPedidoHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AtualizarStatusPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await _repository.ObterPorIdAsync(request.Id);

        if (pedido == null) throw new NotFoundException("Pedido não encontrado");

        pedido.AtualizarStatus(request.NovoStatus);

        await _repository.AtualizarAsync(pedido);
    }
}


public sealed class AtualizarStatusPedidoValidator : AbstractValidator<AtualizarStatusPedidoCommand>
{
    public AtualizarStatusPedidoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.NovoStatus)
            .IsInEnum();
    }
}