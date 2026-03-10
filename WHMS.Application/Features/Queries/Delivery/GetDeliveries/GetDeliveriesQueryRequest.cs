using MediatR;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesQueryRequest : IRequest<GetDeliveriesQueryResponse>
{
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public bool? IsReceived { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}