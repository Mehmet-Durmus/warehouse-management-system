namespace WHMS.Persistence.SeedData;

public class SeedSettings
{
    public AdminData? AdminData { get; set; }
    public LogisticDirectorData? LogisticDirectorData { get; set; }
}

public class AdminData
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

public class LogisticDirectorData
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}