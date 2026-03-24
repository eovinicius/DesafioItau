using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Entities.Enums;
using CatalogoItau.Domain.Exceptions;

using FluentAssertions;

namespace Catalogo.UnitTest.Domain;

public sealed class PedidoTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithPendenteStatus()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var pedido = new Pedido("PED-001", "Cliente", "cliente@email.com");

        // Assert
        pedido.NumeroPedido.Should().Be("PED-001");
        pedido.ClienteNome.Should().Be("Cliente");
        pedido.ClienteEmail.Should().Be("cliente@email.com");
        pedido.Status.Should().Be(StatusPedido.Pendente);
        pedido.DataPedido.Should().BeOnOrAfter(before);
        pedido.Itens.Should().BeEmpty();
    }

    [Fact]
    public void AdicionarItem_ShouldThrow_WhenProdutoIsNull()
    {
        // Arrange
        var pedido = new Pedido("PED-001", "Cliente", "cliente@email.com");
        Produto? produto = null;

        // Act
        Action act = () => pedido.AdicionarItem(produto!, 1);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AdicionarItem_ShouldThrow_WhenQuantidadeIsLessThanOne()
    {
        // Arrange
        var pedido = new Pedido("PED-001", "Cliente", "cliente@email.com");
        var produto = new Produto("SSD", 500m, 10);

        // Act
        Action act = () => pedido.AdicionarItem(produto, 0);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AdicionarItem_ShouldAddItemAndUpdateTotal()
    {
        // Arrange
        var pedido = new Pedido("PED-001", "Cliente", "cliente@email.com");
        var produto = new Produto("SSD", 500m, 10);

        // Act
        pedido.AdicionarItem(produto, 2);

        // Assert
        pedido.Itens.Should().HaveCount(1);
        pedido.ValorTotal.Should().Be(1000m);
    }

    [Fact]
    public void AtualizarStatus_ShouldChangeStatus_WhenPedidoIsNotCancelled()
    {
        // Arrange
        var pedido = new Pedido("PED-001", "Cliente", "cliente@email.com");

        // Act
        pedido.AtualizarStatus(StatusPedido.Enviado);

        // Assert
        pedido.Status.Should().Be(StatusPedido.Enviado);
    }

    [Fact]
    public void AtualizarStatus_ShouldThrow_WhenPedidoIsCancelled()
    {
        // Arrange
        var pedido = new Pedido("PED-001", "Cliente", "cliente@email.com");
        pedido.Cancelar();

        // Act
        Action act = () => pedido.AtualizarStatus(StatusPedido.Enviado);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancelar_ShouldSetStatusToCancelado_WhenPedidoIsNotEntregue()
    {
        // Arrange
        var pedido = new Pedido("PED-001", "Cliente", "cliente@email.com");

        // Act
        pedido.Cancelar();

        // Assert
        pedido.Status.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public void Cancelar_ShouldThrow_WhenPedidoIsEntregue()
    {
        // Arrange
        var pedido = new Pedido("PED-001", "Cliente", "cliente@email.com");
        pedido.AtualizarStatus(StatusPedido.Entregue);

        // Act
        Action act = () => pedido.Cancelar();

        // Assert
        act.Should().Throw<DomainException>();
    }
}
