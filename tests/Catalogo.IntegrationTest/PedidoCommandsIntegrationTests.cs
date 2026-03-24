using Catalogo.IntegrationTest.Infrastructure;

using CatalogoItau.Application.Commands.Pedidos;
using CatalogoItau.Application.Exceptions;
using CatalogoItau.Domain.Entities;
using CatalogoItau.Domain.Entities.Enums;
using CatalogoItau.Domain.Exceptions;
using CatalogoItau.Domain.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.IntegrationTest;

[Collection("Commands Integration")]
public sealed class PedidoCommandsIntegrationTests
{
    private readonly CommandsIntegrationFixture _fixture;

    public PedidoCommandsIntegrationTests(CommandsIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CriarPedidoCommand_ShouldPersistPedidoAndDecreaseStock()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var arrangeScope = _fixture.CreateScope();
        var produtoRepository = arrangeScope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var pedidoRepository = arrangeScope.ServiceProvider.GetRequiredService<IPedidoRepository>();
        var unitOfWork = arrangeScope.ServiceProvider.GetRequiredService<CatalogoItau.Application.Abstractions.Data.IUnitOfWork>();

        var produto = new Produto("Notebook", 5000m, 10);
        await produtoRepository.AdicionarAsync(produto);

        var handler = new CriarPedidoCommand.CriarPedidoHandler(pedidoRepository, produtoRepository, unitOfWork);

        var command = new CriarPedidoCommand
        {
            NumeroPedido = "PED-IT-001",
            ClienteNome = "Cliente Teste",
            ClienteEmail = "cliente@teste.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = produto.Id, Quantidade = 3 }]
        };

        // Act
        var pedidoCriado = await handler.Handle(command, CancellationToken.None);

        // Assert
        using var assertScope = _fixture.CreateScope();
        var produtoRepositoryAssert = assertScope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var pedidoRepositoryAssert = assertScope.ServiceProvider.GetRequiredService<IPedidoRepository>();

        var produtoAtualizado = await produtoRepositoryAssert.ObterPorIdAsync(produto.Id);
        var pedidoPersistido = await pedidoRepositoryAssert.ObterPorIdAsync(pedidoCriado.Id);

        Assert.NotNull(produtoAtualizado);
        Assert.Equal(7, produtoAtualizado.Estoque);

        Assert.NotNull(pedidoPersistido);
        Assert.Equal("PED-IT-001", pedidoPersistido.NumeroPedido);
        Assert.Single(pedidoPersistido.Itens);
        Assert.Equal(15000m, pedidoPersistido.ValorTotal);
    }

    [Fact]
    public async Task CriarPedidoCommand_ShouldThrowDomainException_WhenNumeroPedidoAlreadyExists()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<CatalogoItau.Application.Abstractions.Data.IUnitOfWork>();

        var produto = new Produto("Mouse", 100m, 10);
        await produtoRepository.AdicionarAsync(produto);

        var handler = new CriarPedidoCommand.CriarPedidoHandler(pedidoRepository, produtoRepository, unitOfWork);

        var command = new CriarPedidoCommand
        {
            NumeroPedido = "PED-IT-002",
            ClienteNome = "Cliente",
            ClienteEmail = "cliente@teste.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = produto.Id, Quantidade = 1 }]
        };

        await handler.Handle(command, CancellationToken.None);

        // Act
        Task Act() => handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<DomainException>(Act);
    }

    [Fact]
    public async Task CriarPedidoCommand_ShouldThrowNotFoundException_WhenProdutoDoesNotExist()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<CatalogoItau.Application.Abstractions.Data.IUnitOfWork>();

        var handler = new CriarPedidoCommand.CriarPedidoHandler(pedidoRepository, produtoRepository, unitOfWork);

        var command = new CriarPedidoCommand
        {
            NumeroPedido = "PED-IT-003",
            ClienteNome = "Cliente",
            ClienteEmail = "cliente@teste.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = 9999, Quantidade = 1 }]
        };

        // Act
        Task Act() => handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(Act);

        var pedidoInexistente = await pedidoRepository.ObterPorNumeroAsync("PED-IT-003");
        Assert.Null(pedidoInexistente);
    }

    [Fact]
    public async Task AtualizarStatusPedidoCommand_ShouldPersistStatusChange()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var arrangeScope = _fixture.CreateScope();
        var produtoRepository = arrangeScope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var pedidoRepository = arrangeScope.ServiceProvider.GetRequiredService<IPedidoRepository>();
        var unitOfWork = arrangeScope.ServiceProvider.GetRequiredService<CatalogoItau.Application.Abstractions.Data.IUnitOfWork>();

        var produto = new Produto("Teclado", 250m, 10);
        await produtoRepository.AdicionarAsync(produto);

        var criarHandler = new CriarPedidoCommand.CriarPedidoHandler(pedidoRepository, produtoRepository, unitOfWork);
        var pedido = await criarHandler.Handle(new CriarPedidoCommand
        {
            NumeroPedido = "PED-IT-004",
            ClienteNome = "Cliente",
            ClienteEmail = "cliente@teste.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = produto.Id, Quantidade = 2 }]
        }, CancellationToken.None);

        var atualizarHandler = new AtualizarStatusPedidoHandler(pedidoRepository);

        // Act
        await atualizarHandler.Handle(new AtualizarStatusPedidoCommand
        {
            Id = pedido.Id,
            NovoStatus = StatusPedido.Enviado
        }, CancellationToken.None);

        // Assert
        var pedidoAtualizado = await pedidoRepository.ObterPorIdAsync(pedido.Id);
        Assert.NotNull(pedidoAtualizado);
        Assert.Equal(StatusPedido.Enviado, pedidoAtualizado.Status);
    }

    [Fact]
    public async Task AtualizarStatusPedidoCommand_ShouldThrowNotFoundException_WhenPedidoDoesNotExist()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();
        var handler = new AtualizarStatusPedidoHandler(pedidoRepository);

        // Act
        Task Act() => handler.Handle(new AtualizarStatusPedidoCommand
        {
            Id = 9999,
            NovoStatus = StatusPedido.Enviado
        }, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task CancelarPedidoCommand_ShouldPersistStatusAsCancelado()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<CatalogoItau.Application.Abstractions.Data.IUnitOfWork>();

        var produto = new Produto("Headset", 300m, 8);
        await produtoRepository.AdicionarAsync(produto);

        var criarHandler = new CriarPedidoCommand.CriarPedidoHandler(pedidoRepository, produtoRepository, unitOfWork);
        var pedido = await criarHandler.Handle(new CriarPedidoCommand
        {
            NumeroPedido = "PED-IT-005",
            ClienteNome = "Cliente",
            ClienteEmail = "cliente@teste.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = produto.Id, Quantidade = 1 }]
        }, CancellationToken.None);

        var cancelarHandler = new CancelarPedidoHandler(pedidoRepository);

        // Act
        await cancelarHandler.Handle(new CancelarPedidoCommand { Id = pedido.Id }, CancellationToken.None);

        // Assert
        var pedidoCancelado = await pedidoRepository.ObterPorIdAsync(pedido.Id);
        Assert.NotNull(pedidoCancelado);
        Assert.Equal(StatusPedido.Cancelado, pedidoCancelado.Status);
    }

    [Fact]
    public async Task CancelarPedidoCommand_ShouldThrowDomainException_WhenPedidoIsEntregue()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
        var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<CatalogoItau.Application.Abstractions.Data.IUnitOfWork>();

        var produto = new Produto("Webcam", 450m, 5);
        await produtoRepository.AdicionarAsync(produto);

        var criarHandler = new CriarPedidoCommand.CriarPedidoHandler(pedidoRepository, produtoRepository, unitOfWork);
        var pedido = await criarHandler.Handle(new CriarPedidoCommand
        {
            NumeroPedido = "PED-IT-006",
            ClienteNome = "Cliente",
            ClienteEmail = "cliente@teste.com",
            Itens = [new CriarPedidoCommand.ItemDto { ProdutoId = produto.Id, Quantidade = 1 }]
        }, CancellationToken.None);

        var atualizarHandler = new AtualizarStatusPedidoHandler(pedidoRepository);
        await atualizarHandler.Handle(new AtualizarStatusPedidoCommand
        {
            Id = pedido.Id,
            NovoStatus = StatusPedido.Entregue
        }, CancellationToken.None);

        var cancelarHandler = new CancelarPedidoHandler(pedidoRepository);

        // Act
        Task Act() => cancelarHandler.Handle(new CancelarPedidoCommand { Id = pedido.Id }, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<DomainException>(Act);
    }
}
