using MediatR;

namespace WHMS.Application.Features.Queries.Shipment.GetShipments;

public class GetShipmentsQueryRequest : IRequest<GetShipmentsQueryResponse>
{
    public string? WarehouseId { get; set; }
    public string? WarehouseCityId { get; set; }
    public string? WarehouseDistrictId { get; set; }
    public string? WarehouseNeighborhoodId { get; set; }
    public string? StoreId { get; set; }
    public string? StoreCityId { get; set; }
    public string? StoreDistrictId { get; set; }
    public string? StoreNeighborhoodId { get; set; }
    public List<string>? SkuIds { get; set; }
    public bool? IsSent { get; set; }
    public DateTime? SentAfter { get; set; }
    public DateTime? SentBefore { get; set; }
    public DateTime? ExpectedSendingDateAfter { get; set; }
    public DateTime? ExpectedSendingDateBefore { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}