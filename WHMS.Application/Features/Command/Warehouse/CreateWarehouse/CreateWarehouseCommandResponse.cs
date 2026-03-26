namespace WHMS.Application.Features.Command.Warehouse.CreateWarehouse;

public class CreateWarehouseCommandResponse
{
    public CreateWarehouseResultDto ResultDto { get; set; } = null!;
    public List<string> Warnings { get; set; } = null!;
}