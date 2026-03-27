namespace WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;

public class UpdateWarehouseCommandResponse
{
    public string WarehouseId { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public string Address { get; set; } = null!;
}