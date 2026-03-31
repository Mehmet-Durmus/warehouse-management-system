using WHMS.Application.Common.DTOs;
using WHMS.Application.DTOs.Shipment;

namespace WHMS.Application.Features.Queries.Shipment.GetShipments;

public class GetShipmentsQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetShipmentsResultShipmentDto> Shipments { get; set; } = null!;
}