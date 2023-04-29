namespace PokemonApi.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder AddGlobalExceptionHandling(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
}