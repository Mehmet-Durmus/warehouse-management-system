using MediatR;

namespace WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;

public class UpdateWarehouseCommandRequest : IRequest<UpdateWarehouseCommandResponse>
{
    public required string WarehouseId { get; set; }
    public required string CityId { get; set; }
    public required string DistrictId { get; set; }
    public required string NeighborhoodId { get; set; }
    public required string AddressLine { get; set; }
    public required string PostalCode { get; set; }
}