using WHMS.Application.Common.DTOs;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetDeliveriesResultDeliveryDto> Deliveries { get; set; } = null!;
}