using CatalogoItau.Application.Abstractions.Pagination;
using CatalogoItau.Application.Queries.Produtos;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using FluentAssertions;

using Moq;

namespace Catalogo.UnitTest.Application.Queries;

public sealed class ProdutoQueryHandlersTests
{
    [Fact]
    public async Task GetProdutoByIdHandler_ShouldReturnProdutoFromRepository()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        var produto = new Produto("Mouse", 100m, 10);

        repositoryMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(produto);

        var handler = new GetProdutoByIdHandler(repositoryMock.Object);
        var query = new GetProdutoByIdQuery { Id = 1 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(produto);
    }

    [Fact]
    public async Task GetProdutosHandler_ShouldReturnAtivosFromRepository()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        var produtos = new List<Produto>
        {
            new("Mouse", 100m, 10),
            new("Teclado", 200m, 5)
        };

        repositoryMock.Setup(x => x.ObterAtivosAsync()).ReturnsAsync(produtos);

        var handler = new GetProdutosHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetProdutosQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(produtos);
    }

    [Fact]
    public async Task GetProdutosPaginadosHandler_ShouldReturnPagedResult()
    {
        // Arrange
        var repositoryMock = new Mock<IProdutoRepository>();
        var produtos = new List<Produto> { new("Mouse", 100m, 10) };

        repositoryMock
            .Setup(x => x.ObterAtivosComPaginacaoAsync(2, 5))
            .ReturnsAsync((produtos, 12));

        var handler = new GetProdutosPaginadosHandler(repositoryMock.Object);
        var query = new GetProdutosPaginadosQuery(2, 5);

        // Act
        PagedResult<Produto> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEquivalentTo(produtos);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalItems.Should().Be(12);
    }

    [Fact]
    public void GetProdutosPaginadosQuery_ShouldNormalizeInvalidValues()
    {
        // Arrange

        // Act
        var query = new GetProdutosPaginadosQuery(0, 500);

        // Assert
        query.Page.Should().Be(1);
        query.PageSize.Should().Be(10);
    }
}
