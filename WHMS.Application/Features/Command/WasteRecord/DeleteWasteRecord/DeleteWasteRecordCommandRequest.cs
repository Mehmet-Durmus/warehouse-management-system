using MediatR;

namespace WHMS.Application.Features.Command.WasteRecord.DeleteWasteRecord;

public class DeleteWasteRecordCommandRequest : IRequest<DeleteWasteRecordCommandResponse>
{
    public string? WasteRecordId { get; set; }
}