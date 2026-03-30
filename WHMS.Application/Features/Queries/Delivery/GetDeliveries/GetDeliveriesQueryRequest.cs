using MediatR;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesQueryRequest : IRequest<GetDeliveriesQueryResponse>
{
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public List<string>? SkuIds { get; set; }
    public bool? IsReceived { get; set; }
    public DateTime? ReceivedAfter { get; set; }
    public DateTime? ReceivedBefore { get; set; }
    public DateTime? ExpectedArrivalAfter { get; set; }
    public DateTime? ExpectedArrivalBefore { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}