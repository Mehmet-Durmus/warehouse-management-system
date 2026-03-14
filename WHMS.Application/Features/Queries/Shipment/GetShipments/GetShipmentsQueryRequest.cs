using MediatR;

namespace WHMS.Application.Features.Queries.Shipment.GetShipments;

public class GetShipmentsQueryRequest : IRequest<GetShipmentsQueryResponse>
{
    public string? WarehouseId { get; set; }
    public string? WarehouseCityId { get; set; }
    public string? WarehouseDistrictId { get; set; }
    public string? StoreId { get; set; }
    public string? StoreCityId { get; set; }
    public string? StoreDistrictId { get; set; }
    public bool? IsSent { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}