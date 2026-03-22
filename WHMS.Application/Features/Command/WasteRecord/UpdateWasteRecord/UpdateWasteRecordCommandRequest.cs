using MediatR;

namespace WHMS.Application.Features.Command.WasteRecord.UpdateWasteRecord;

public class UpdateWasteRecordCommandRequest : IRequest<UpdateWasteRecordCommandResponse>
{
    public string? WasteRecordId { get; set; }
    public string? WarehouseId { get; set; }
    public string? SkuId { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
}