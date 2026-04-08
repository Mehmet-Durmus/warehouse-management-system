using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WHMS.Application.Authorization.PasswordChanged;
using WHMS.Application.Authorization.WarehouseAssigned;
using WHMS.Application.Behaviors;
using WHMS.Application.Configurations;

namespace WHMS.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
        services.AddSingleton<IAuthorizationHandler, WarehouseAssignedHandler>();
        services.AddScoped<IAuthorizationHandler, PasswordChangedHandler>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


        


        return services;
    }
}