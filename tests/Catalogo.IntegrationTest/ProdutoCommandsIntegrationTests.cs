using Catalogo.IntegrationTest.Infrastructure;

using CatalogoItau.Application.Commands.Produtos;
using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.IntegrationTest;

[Collection("Commands Integration")]
public sealed class ProdutoCommandsIntegrationTests
{
    private readonly CommandsIntegrationFixture _fixture;

    public ProdutoCommandsIntegrationTests(CommandsIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CriarProdutoCommand_ShouldPersistProduto()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var handler = new CriarProdutoHandler(produtoRepository);

        var command = new CriarProdutoCommand("Produto IT", "Descricao", 199.90m, 12);

        // Act
        var produtoId = await handler.Handle(command, CancellationToken.None);

        // Assert
        var produto = await produtoRepository.ObterPorIdAsync(produtoId);

        Assert.NotNull(produto);
        Assert.Equal("Produto IT", produto.Nome);
        Assert.Equal("Descricao", produto.Descricao);
        Assert.Equal(199.90m, produto.Preco);
        Assert.Equal(12, produto.Estoque);
        Assert.True(produto.Ativo);
    }

    [Fact]
    public async Task AtualizarProdutoCommand_ShouldPersistUpdatedValues()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

        var produto = new Produto("Produto Original", 100m, 5, "Antiga");
        await produtoRepository.AdicionarAsync(produto);

        var handler = new AtualizarProdutoHandler(produtoRepository);

        var command = new AtualizarProdutoCommand
        {
            Id = produto.Id,
            Nome = "Produto Atualizado",
            Descricao = "Nova",
            Preco = 250m,
            Estoque = 8
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var produtoAtualizado = await produtoRepository.ObterPorIdAsync(produto.Id);

        Assert.NotNull(produtoAtualizado);
        Assert.Equal("Produto Atualizado", produtoAtualizado.Nome);
        Assert.Equal("Nova", produtoAtualizado.Descricao);
        Assert.Equal(250m, produtoAtualizado.Preco);
        Assert.Equal(8, produtoAtualizado.Estoque);
    }

    [Fact]
    public async Task AtualizarProdutoCommand_ShouldThrowNotFoundException_WhenProdutoDoesNotExist()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var handler = new AtualizarProdutoHandler(produtoRepository);

        var command = new AtualizarProdutoCommand
        {
            Id = 9999,
            Nome = "Nao existe",
            Preco = 10m,
            Estoque = 1
        };

        // Act
        Task Act() => handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task AtualizarProdutoCommand_ShouldThrowNotFoundException_WhenProdutoIsInactive()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

        var produto = new Produto("Produto Inativo", 80m, 3);
        produto.Inativar();
        await produtoRepository.AdicionarAsync(produto);

        var handler = new AtualizarProdutoHandler(produtoRepository);

        var command = new AtualizarProdutoCommand
        {
            Id = produto.Id,
            Nome = "Tentativa",
            Preco = 90m,
            Estoque = 4
        };

        // Act
        Task Act() => handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task DeletarProdutoCommand_ShouldSetProdutoAsInactive()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

        var produto = new Produto("Produto Delete", 500m, 2);
        await produtoRepository.AdicionarAsync(produto);

        var handler = new DeletarProdutoHandler(produtoRepository);

        // Act
        await handler.Handle(new DeletarProdutoCommand { Id = produto.Id }, CancellationToken.None);

        // Assert
        var produtoAtualizado = await produtoRepository.ObterPorIdAsync(produto.Id);

        Assert.NotNull(produtoAtualizado);
        Assert.False(produtoAtualizado.Ativo);
    }

    [Fact]
    public async Task DeletarProdutoCommand_ShouldThrowNotFoundException_WhenProdutoDoesNotExist()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var handler = new DeletarProdutoHandler(produtoRepository);

        // Act
        Task Act() => handler.Handle(new DeletarProdutoCommand { Id = 9999 }, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task DeletarProdutoCommand_ShouldThrowNotFoundException_WhenProdutoIsInactive()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

        var produto = new Produto("Produto Ja Inativo", 120m, 6);
        produto.Inativar();
        await produtoRepository.AdicionarAsync(produto);

        var handler = new DeletarProdutoHandler(produtoRepository);

        // Act
        Task Act() => handler.Handle(new DeletarProdutoCommand { Id = produto.Id }, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }
}
