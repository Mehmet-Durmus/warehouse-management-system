using WHMS.Application.Common.DTOs;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItems;

public class GetShipmentItemsQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetShipmentItemsResultShipmentItemDto> ShipmentItems { get; set; } = null!;
}