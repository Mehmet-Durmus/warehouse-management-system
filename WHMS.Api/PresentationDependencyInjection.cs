using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WHMS.Api.Common.ExceptionHandling;
using WHMS.Application.Authorization.PasswordChanged;
using WHMS.Application.Authorization.WarehouseAssigned;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Command.Auth.Login;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Api;

public static class PresentationDependencyInjection
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddProblemDetails();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<WHMSDbContext>()
        .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            var warehouseAssigned = new WarehouseAssignedRequirement();
            var passwordChanged = new PasswordChangedRequirement();
            var LogisticDirector = ApplicationRole.LogisticDirector;
            var WarehouseManager = ApplicationRole.WarehouseManager;
            var WarehouseStaff = ApplicationRole.WarehouseStaff;
            options.AddPolicy("LogisticDirector", p => 
                p.RequireRole(LogisticDirector).AddRequirements(passwordChanged));
            options.AddPolicy("WarehouseManager", p => 
                p.RequireRole(WarehouseManager).AddRequirements(warehouseAssigned).AddRequirements(passwordChanged));
            options.AddPolicy("WarehouseStaff", p => 
                p.RequireRole(WarehouseStaff).AddRequirements(warehouseAssigned).AddRequirements(passwordChanged));
            options.AddPolicy("DirectorOrManager", p => 
                p.RequireRole(LogisticDirector, WarehouseManager).AddRequirements(warehouseAssigned).AddRequirements(passwordChanged));
            options.AddPolicy("ManagerOrStaff", p => 
                p.RequireRole(WarehouseManager, WarehouseStaff).AddRequirements(warehouseAssigned).AddRequirements(passwordChanged));
            options.AddPolicy("DirectorManagerOrStaff", p => 
                p.RequireRole(LogisticDirector, WarehouseManager, WarehouseStaff).AddRequirements(warehouseAssigned).AddRequirements(passwordChanged));
            options.AddPolicy("PasswordChange", p => 
                p.RequireRole(LogisticDirector, WarehouseManager, WarehouseStaff));
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header. Example: \"Bearer {token}\""
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        return services;
    }
}