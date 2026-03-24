using CatalogoItau.Application.Abstractions.Pagination;
using CatalogoItau.Application.Queries.Pedidos;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using FluentAssertions;

using Moq;

namespace Catalogo.UnitTest.Application.Queries;

public sealed class PedidoQueryHandlersTests
{
    [Fact]
    public async Task GetPedidoByIdHandler_ShouldReturnPedidoFromRepository()
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        var pedido = new Pedido("PED-1", "Cliente", "cliente@email.com");

        repositoryMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(pedido);

        var handler = new GetPedidoByIdHandler(repositoryMock.Object);
        var query = new GetPedidoByIdQuery { Id = 1 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(pedido);
    }

    [Fact]
    public async Task GetPedidosHandler_ShouldReturnPedidosFromRepository()
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        var pedidos = new List<Pedido>
        {
            new("PED-1", "Cliente 1", "c1@email.com"),
            new("PED-2", "Cliente 2", "c2@email.com")
        };

        repositoryMock.Setup(x => x.ObterTodosAsync()).ReturnsAsync(pedidos);

        var handler = new GetPedidosHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetPedidosQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(pedidos);
    }

    [Fact]
    public async Task GetPedidosPaginadosHandler_ShouldReturnPagedResult()
    {
        // Arrange
        var repositoryMock = new Mock<IPedidoRepository>();
        var pedidos = new List<Pedido> { new("PED-1", "Cliente", "c@email.com") };

        repositoryMock
            .Setup(x => x.ObterTodosComPaginacaoAsync(2, 5))
            .ReturnsAsync((pedidos, 17));

        var handler = new GetPedidosPaginadosHandler(repositoryMock.Object);
        var query = new GetPedidosPaginadosQuery(2, 5);

        // Act
        PagedResult<Pedido> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEquivalentTo(pedidos);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalItems.Should().Be(17);
    }

    [Fact]
    public void GetPedidosPaginadosQuery_ShouldNormalizeInvalidValues()
    {
        // Arrange

        // Act
        var query = new GetPedidosPaginadosQuery(-1, 999);

        // Assert
        query.Page.Should().Be(1);
        query.PageSize.Should().Be(10);
    }
}
