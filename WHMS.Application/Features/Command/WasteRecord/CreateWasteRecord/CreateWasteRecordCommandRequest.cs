using MediatR;

namespace WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;

public class CreateWasteRecordCommandRequest : IRequest<CreateWasteRecordCommandResponse>
{
    public string? WarehouseId { get; set; }
    public string? SkuId { get; set; }
    public int? Quantity { get; set; }
    public string? Description { get; set; }
}