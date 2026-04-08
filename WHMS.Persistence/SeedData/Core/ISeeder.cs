namespace WHMS.Persistence.SeedData.Core;

public interface ISeeder
{
    public int Order { get; }
    Task SeedAsync();
}