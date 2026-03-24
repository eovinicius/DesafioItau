using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Exceptions;

using FluentAssertions;

namespace Catalogo.UnitTest.Domain;

public sealed class ItemPedidoTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenProdutoIsNull()
    {
        // Arrange
        Produto? produto = null;

        // Act
        Action act = () => _ = new ItemPedido(produto!, 1);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenQuantidadeIsLessThanOne()
    {
        // Arrange
        var produto = new Produto("Headset", 300m, 10);

        // Act
        Action act = () => _ = new ItemPedido(produto, 0);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_ShouldCreateItemAndCalculateSubtotal()
    {
        // Arrange
        var produto = new Produto("Headset", 300m, 10);

        // Act
        var item = new ItemPedido(produto, 3);

        // Assert
        item.Quantidade.Should().Be(3);
        item.PrecoUnitario.Should().Be(300m);
        item.Subtotal.Should().Be(900m);
    }
}
