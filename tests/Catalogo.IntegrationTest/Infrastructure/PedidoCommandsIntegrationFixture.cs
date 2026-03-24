using CatalogoItau.Application.Abstractions.Data;
using CatalogoItau.Domain.Repositories;
using CatalogoItau.Infrastructure.Percistence;
using CatalogoItau.Infrastructure.Percistence.Data;
using CatalogoItau.Infrastructure.Percistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.IntegrationTest.Infrastructure;

public sealed class CommandsIntegrationFixture : IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;

    public CommandsIntegrationFixture()
    {
        var connectionString = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=CatalogoItau;Username=admin;Password=postgres@Itau123";

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        _serviceProvider.Dispose();
        return Task.CompletedTask;
    }

    public IServiceScope CreateScope() => _serviceProvider.CreateScope();

    public async Task ResetDatabaseAsync()
    {
        using var scope = CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
