namespace WHMS.Application.Common.Constants;

public static class ApplicationRole
{
    public const string LogisticDirector = "LogisticDirector";
    public const string WarehouseManager = "WarehouseManager";
    public const string WarehouseStaff = "WarehouseStaff";
    public static IEnumerable<string> All =>
    [
        LogisticDirector,
        WarehouseManager,
        WarehouseStaff
    ];
}