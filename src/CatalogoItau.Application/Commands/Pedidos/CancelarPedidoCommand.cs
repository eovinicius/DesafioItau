using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Repositories;

using MediatR;

namespace CatalogoItau.Application.Commands.Pedidos;

public sealed class CancelarPedidoCommand : IRequest
{
    public int Id { get; set; }
}

public sealed class CancelarPedidoHandler : IRequestHandler<CancelarPedidoCommand>
{
    private readonly IPedidoRepository _repository;

    public CancelarPedidoHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CancelarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await _repository.ObterPorIdAsync(request.Id);

        if (pedido == null) throw new NotFoundException("Pedido não encontrado");

        pedido.Cancelar();

        await _repository.AtualizarAsync(pedido);
    }
}