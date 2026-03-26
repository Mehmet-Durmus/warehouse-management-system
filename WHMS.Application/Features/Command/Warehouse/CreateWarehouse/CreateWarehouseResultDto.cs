namespace WHMS.Application.Features.Command.Warehouse.CreateWarehouse;

public class CreateWarehouseResultDto
{
    public string WarehouseId { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public string Address { get; set; } = null!;
}