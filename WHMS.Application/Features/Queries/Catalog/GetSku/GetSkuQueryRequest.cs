using MediatR;

namespace WHMS.Application.Features.Queries.Catalog.GetSku;

public class GetSkuQueryRequest : IRequest<GetSkuQueryResponse>
{
    public string? SkuId { get; set; }
}