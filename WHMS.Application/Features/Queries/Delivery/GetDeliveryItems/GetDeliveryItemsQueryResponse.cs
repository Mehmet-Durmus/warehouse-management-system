using WHMS.Application.Common.DTOs;
using WHMS.Application.DTOs.Delivery;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;

public class GetDeliveryItemsQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetDeliveryItemsResultDeliveryItemDto> DeliveryItems { get; set; } = null!;
}