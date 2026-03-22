using MediatR;

namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecord;

public class GetWasteRecordQueryRequest : IRequest<GetWasteRecordQueryResponse>
{
    public string? WasteRecordId { get; set; }
}