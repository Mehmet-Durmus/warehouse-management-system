using WHMS.Persistence.SeedData.Seeders;

namespace WHMS.Persistence.SeedData.Core;

public class SeedRunner
{
    private readonly IEnumerable<ISeeder> _seeders;

    public SeedRunner(IEnumerable<ISeeder> seeders)
    {
        _seeders = seeders;
    }

    public async Task RunAsync()
    {
        foreach (var seeder in _seeders.OrderBy(s => s.Order))
            await seeder.SeedAsync();
    }
}