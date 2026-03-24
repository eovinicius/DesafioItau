using CatalogoItau.Application.Commands.Produtos;
using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using FluentAssertions;

using Moq;

namespace Catalogo.UnitTest.Application.Commands;

public sealed class ProdutosCommandHandlersTests
{
    [Fact]
    public async Task CriarProdutoHandler_ShouldAddProdutoAndReturnId()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        Produto? capturedProduto = null;

        repositoryMock
            .Setup(x => x.AdicionarAsync(It.IsAny<Produto>()))
            .Callback<Produto>(produto => capturedProduto = produto)
            .Returns(Task.CompletedTask);

        var handler = new CriarProdutoHandler(repositoryMock.Object);
        var command = new CriarProdutoCommand("Mouse", "Sem fio", 120m, 8);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        capturedProduto.Should().NotBeNull();
        capturedProduto!.Nome.Should().Be("Mouse");
        capturedProduto.Descricao.Should().Be("Sem fio");
        capturedProduto.Preco.Should().Be(120m);
        capturedProduto.Estoque.Should().Be(8);
        result.Should().Be(capturedProduto.Id);

        repositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarProdutoHandler_ShouldThrowNotFound_WhenProdutoDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        repositoryMock.Setup(x => x.ObterPorIdAsync(10)).ReturnsAsync((Produto?)null);

        var handler = new AtualizarProdutoHandler(repositoryMock.Object);
        var command = new AtualizarProdutoCommand
        {
            Id = 10,
            Nome = "Teclado",
            Preco = 300m,
            Estoque = 4,
            Descricao = "Mecanico"
        };

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarProdutoHandler_ShouldThrowNotFound_WhenProdutoIsInactive()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        var produto = new Produto("Teclado", 200m, 5);
        produto.Inativar();

        repositoryMock.Setup(x => x.ObterPorIdAsync(10)).ReturnsAsync(produto);

        var handler = new AtualizarProdutoHandler(repositoryMock.Object);
        var command = new AtualizarProdutoCommand
        {
            Id = 10,
            Nome = "Teclado 2",
            Preco = 250m,
            Estoque = 6,
            Descricao = "Novo"
        };

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarProdutoHandler_ShouldUpdateProduto_WhenProdutoExistsAndIsActive()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        var produto = new Produto("Monitor", 1000m, 3, "27");

        repositoryMock.Setup(x => x.ObterPorIdAsync(5)).ReturnsAsync(produto);
        repositoryMock.Setup(x => x.AtualizarAsync(produto)).Returns(Task.CompletedTask);

        var handler = new AtualizarProdutoHandler(repositoryMock.Object);
        var command = new AtualizarProdutoCommand
        {
            Id = 5,
            Nome = "Monitor Pro",
            Preco = 1200m,
            Estoque = 7,
            Descricao = "32"
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        produto.Nome.Should().Be("Monitor Pro");
        produto.Preco.Should().Be(1200m);
        produto.Estoque.Should().Be(7);
        produto.Descricao.Should().Be("32");

        repositoryMock.Verify(x => x.AtualizarAsync(produto), Times.Once);
    }

    [Fact]
    public async Task DeletarProdutoHandler_ShouldThrowNotFound_WhenProdutoDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        repositoryMock.Setup(x => x.ObterPorIdAsync(2)).ReturnsAsync((Produto?)null);

        var handler = new DeletarProdutoHandler(repositoryMock.Object);
        var command = new DeletarProdutoCommand { Id = 2 };

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task DeletarProdutoHandler_ShouldInactivateProduto_WhenProdutoExistsAndIsActive()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        var produto = new Produto("Mesa", 600m, 2);

        repositoryMock.Setup(x => x.ObterPorIdAsync(2)).ReturnsAsync(produto);
        repositoryMock.Setup(x => x.AtualizarAsync(produto)).Returns(Task.CompletedTask);

        var handler = new DeletarProdutoHandler(repositoryMock.Object);
        var command = new DeletarProdutoCommand { Id = 2 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        produto.Ativo.Should().BeFalse();
        repositoryMock.Verify(x => x.AtualizarAsync(produto), Times.Once);
    }
}
