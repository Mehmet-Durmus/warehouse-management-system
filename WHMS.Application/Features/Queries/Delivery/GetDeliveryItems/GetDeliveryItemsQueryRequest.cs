using MediatR;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;

public class GetDeliveryItemsQueryRequest : IRequest<GetDeliveryItemsQueryResponse>
{
    public string DeliveryId { get; set; } = null!;
    public string? SkuId { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinQuantity { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}