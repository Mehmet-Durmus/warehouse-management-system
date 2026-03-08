using MediatR;

namespace WHMS.Application.Features.Queries.Store.GetStore;

public class GetStoreQueryRequest : IRequest<GetStoreQueryResponse>
{
    public string? StoreId { get; set; }
}