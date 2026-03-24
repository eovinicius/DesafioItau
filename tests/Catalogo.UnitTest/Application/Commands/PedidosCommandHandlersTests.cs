using CatalogoItau.Application.Abstractions.Data;
using CatalogoItau.Application.Commands.Pedidos;
using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Entities.Enums;
using CatalogoItau.Domain.Exceptions;
using CatalogoItau.Domain.Repositories;

using FluentAssertions;

using Moq;

namespace Catalogo.UnitTest.Application.Commands;

public sealed class PedidosCommandHandlersTests
{
    [Fact]
    public async Task AtualizarStatusPedidoHandler_ShouldThrowNotFound_WhenPedidoDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        repositoryMock.Setup(x => x.ObterPorIdAsync(20)).ReturnsAsync((Pedido?)null);

        var handler = new AtualizarStatusPedidoHandler(repositoryMock.Object);
        var command = new AtualizarStatusPedidoCommand { Id = 20, NovoStatus = StatusPedido.Enviado };

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarStatusPedidoHandler_ShouldUpdateStatus_WhenPedidoExists()
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        var pedido = new Pedido("PED-100", "Cliente", "cliente@email.com");

        repositoryMock.Setup(x => x.ObterPorIdAsync(20)).ReturnsAsync(pedido);
        repositoryMock.Setup(x => x.AtualizarAsync(pedido)).Returns(Task.CompletedTask);

        var handler = new AtualizarStatusPedidoHandler(repositoryMock.Object);
        var command = new AtualizarStatusPedidoCommand { Id = 20, NovoStatus = StatusPedido.Enviado };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        pedido.Status.Should().Be(StatusPedido.Enviado);
        repositoryMock.Verify(x => x.AtualizarAsync(pedido), Times.Once);
    }

    [Fact]
    public async Task CancelarPedidoHandler_ShouldThrowNotFound_WhenPedidoDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        repositoryMock.Setup(x => x.ObterPorIdAsync(30)).ReturnsAsync((Pedido?)null);

        var handler = new CancelarPedidoHandler(repositoryMock.Object);
        var command = new CancelarPedidoCommand { Id = 30 };

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task CancelarPedidoHandler_ShouldCancelPedido_WhenPedidoExists()
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        var pedido = new Pedido("PED-101", "Cliente", "cliente@email.com");

        repositoryMock.Setup(x => x.ObterPorIdAsync(30)).ReturnsAsync(pedido);
        repositoryMock.Setup(x => x.AtualizarAsync(pedido)).Returns(Task.CompletedTask);

        var handler = new CancelarPedidoHandler(repositoryMock.Object);
        var command = new CancelarPedidoCommand { Id = 30 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        pedido.Status.Should().Be(StatusPedido.Cancelado);
        repositoryMock.Verify(x => x.AtualizarAsync(pedido), Times.Once);
    }

    [Fact]
    public async Task CriarPedidoHandler_ShouldThrowDomainException_WhenNumeroPedidoAlreadyExists()
    {
        // Arrange
        var pedidoRepositoryMock = new Mock<IPedidoRepository>();
        var produtoRepositoryMock = new Mock<IProdutoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var existingPedido = new Pedido("PED-EXIST", "Cliente", "email@x.com");
        pedidoRepositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-EXIST")).ReturnsAsync(existingPedido);

        var handler = new CriarPedidoCommand.CriarPedidoHandler(
            pedidoRepositoryMock.Object,
            produtoRepositoryMock.Object,
            unitOfWorkMock.Object);

        var command = new CriarPedidoCommand
        {
            NumeroPedido = "PED-EXIST",
            ClienteNome = "Cliente",
            ClienteEmail = "cliente@email.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = 1, Quantidade = 1 }]
        };

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
        unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Never);
        unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        unitOfWorkMock.Verify(x => x.Rollback(), Times.Never);
    }

    [Fact]
    public async Task CriarPedidoHandler_ShouldRollbackAndThrow_WhenProdutoIsNotFound()
    {
        // Arrange
        var pedidoRepositoryMock = new Mock<IPedidoRepository>();
        var produtoRepositoryMock = new Mock<IProdutoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        pedidoRepositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-200")).ReturnsAsync((Pedido?)null);
        produtoRepositoryMock.Setup(x => x.ObterPorIdParaAtualizacaoAsync(1)).ReturnsAsync((Produto?)null);

        var handler = new CriarPedidoCommand.CriarPedidoHandler(
            pedidoRepositoryMock.Object,
            produtoRepositoryMock.Object,
            unitOfWorkMock.Object);

        var command = new CriarPedidoCommand
        {
            NumeroPedido = "PED-200",
            ClienteNome = "Cliente",
            ClienteEmail = "cliente@email.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = 1, Quantidade = 2 }]
        };

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Once);
        unitOfWorkMock.Verify(x => x.Rollback(), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CriarPedidoHandler_ShouldCreatePedidoCommitTransactionAndDecreaseStock_WhenRequestIsValid()
    {
        // Arrange
        var pedidoRepositoryMock = new Mock<IPedidoRepository>();
        var produtoRepositoryMock = new Mock<IProdutoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        pedidoRepositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-300")).ReturnsAsync((Pedido?)null);

        var produto = new Produto("Mouse", 100m, 10);
        produtoRepositoryMock.Setup(x => x.ObterPorIdParaAtualizacaoAsync(1)).ReturnsAsync(produto);

        Pedido? capturedPedido = null;
        pedidoRepositoryMock
            .Setup(x => x.AdicionarAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(pedido => capturedPedido = pedido)
            .Returns(Task.CompletedTask);

        var handler = new CriarPedidoCommand.CriarPedidoHandler(
            pedidoRepositoryMock.Object,
            produtoRepositoryMock.Object,
            unitOfWorkMock.Object);

        var command = new CriarPedidoCommand
        {
            NumeroPedido = "PED-300",
            ClienteNome = "Cliente",
            ClienteEmail = "cliente@email.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = 1, Quantidade = 3 }]
        };

        // Act
        var pedido = await handler.Handle(command, CancellationToken.None);

        // Assert
        pedido.Should().NotBeNull();
        pedido.Itens.Should().HaveCount(1);
        pedido.ValorTotal.Should().Be(300m);
        produto.Estoque.Should().Be(7);
        capturedPedido.Should().BeSameAs(pedido);

        unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        unitOfWorkMock.Verify(x => x.Rollback(), Times.Never);
        pedidoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Pedido>()), Times.Once);
    }
}
