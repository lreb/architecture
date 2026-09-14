using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Facware.ModularMonolith.Api.DependencyInjection;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddBaseHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API is running."));

        return services;
    }
}