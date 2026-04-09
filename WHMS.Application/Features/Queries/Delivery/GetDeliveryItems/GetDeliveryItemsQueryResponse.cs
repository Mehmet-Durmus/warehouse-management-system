using WHMS.Application.Common.DTOs;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;

public class GetDeliveryItemsQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetDeliveryItemsResultDeliveryItemDto> DeliveryItems { get; set; } = null!;
}