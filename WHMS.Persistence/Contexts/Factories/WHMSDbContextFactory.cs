using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using WHMS.Application.Abstractions.Infrastructure;

namespace WHMS.Persistence.Contexts.Factories;

public class WHMSDbContextFactory : IDesignTimeDbContextFactory<WHMSDbContext>
{
    public WHMSDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets(Assembly.Load("WHMS.Api"), optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<WHMSDbContext>();

        var connectionString = configuration.GetConnectionString("PostgreSQL");
        optionsBuilder.UseNpgsql(connectionString);

        var nullCurrentUserService = new NullCurrnetUserService(); 

        return new WHMSDbContext(optionsBuilder.Options, nullCurrentUserService);
    }

    public class NullCurrnetUserService : ICurrentUserService
    {
        public Guid? UserId => null;

        public string? FullName => null;

        public string? UserName => null;

        public string? WarehouseId => null;
    }
}