using Microsoft.Extensions.DependencyInjection;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Infrastructure.Services;

namespace WHMS.Infrastructure;

public static class InfrastructureDependenyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordCreator, PasswordCreator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IJsonDataLoader, JsonDataLoader>();

        return services;
    }
}