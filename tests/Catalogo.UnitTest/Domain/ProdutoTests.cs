using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Exceptions;

using FluentAssertions;

namespace Catalogo.UnitTest.Domain;

public sealed class ProdutoTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultState()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var produto = new Produto("Notebook", 5000m, 10, "16GB RAM");

        // Assert
        produto.Nome.Should().Be("Notebook");
        produto.Preco.Should().Be(5000m);
        produto.Estoque.Should().Be(10);
        produto.Descricao.Should().Be("16GB RAM");
        produto.Ativo.Should().BeTrue();
        produto.DataCriacao.Should().BeOnOrAfter(before);
        produto.AtualizadoEm.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void Atualizar_ShouldUpdateFieldsAndTimestamp()
    {
        // Arrange
        var produto = new Produto("Mouse", 100m, 10, "Original");
        var previousUpdatedAt = produto.AtualizadoEm;

        Thread.Sleep(5);

        // Act
        produto.Atualizar("Mouse Gamer", 200m, 20, "RGB");

        // Assert
        produto.Nome.Should().Be("Mouse Gamer");
        produto.Preco.Should().Be(200m);
        produto.Estoque.Should().Be(20);
        produto.Descricao.Should().Be("RGB");
        produto.AtualizadoEm.Should().BeAfter(previousUpdatedAt);
    }

    [Fact]
    public void Inativar_ShouldSetAtivoToFalse()
    {
        // Arrange
        var produto = new Produto("Teclado", 250m, 5);

        // Act
        produto.Inativar();

        // Assert
        produto.Ativo.Should().BeFalse();
    }

    [Fact]
    public void BaixarEstoque_ShouldThrow_WhenQuantidadeIsLessThanOne()
    {
        // Arrange
        var produto = new Produto("Monitor", 900m, 5);

        // Act
        Action act = () => produto.BaixarEstoque(0);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void BaixarEstoque_ShouldThrow_WhenEstoqueIsInsufficient()
    {
        // Arrange
        var produto = new Produto("Monitor", 900m, 2);

        // Act
        Action act = () => produto.BaixarEstoque(3);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void BaixarEstoque_ShouldDecreaseStock_WhenQuantidadeIsValid()
    {
        // Arrange
        var produto = new Produto("Cadeira", 700m, 10);
        var previousUpdatedAt = produto.AtualizadoEm;

        Thread.Sleep(5);

        // Act
        produto.BaixarEstoque(4);

        // Assert
        produto.Estoque.Should().Be(6);
        produto.AtualizadoEm.Should().BeAfter(previousUpdatedAt);
    }
}
