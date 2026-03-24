namespace Catalogo.IntegrationTest.Infrastructure;

[CollectionDefinition("Commands Integration", DisableParallelization = true)]
public sealed class CommandsIntegrationCollection : ICollectionFixture<CommandsIntegrationFixture>
{
}
