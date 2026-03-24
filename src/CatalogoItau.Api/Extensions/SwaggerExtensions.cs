using Microsoft.OpenApi;

namespace CatalogoItau.Api.Extensions;

internal static class SwaggerExtensions
{
    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CATALOGO ITAÚ API",
                Version = "v1",
                Description = "API para gerenciamento de catálogo de produtos e pedidos do Itaú.",
            });

            options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
        });

        return services;
    }
}
