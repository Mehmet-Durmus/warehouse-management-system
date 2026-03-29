using WHMS.Application.Common.DTOs;
using WHMS.Application.DTOs.Delivery;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetDeliveriesResultDeliveryDto> Deliveries { get; set; } = null!;
}