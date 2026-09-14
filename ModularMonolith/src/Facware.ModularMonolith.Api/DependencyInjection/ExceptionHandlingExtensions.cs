using Facware.ModularMonolith.Api.Middleware;

namespace Facware.ModularMonolith.Api.DependencyInjection;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }
}