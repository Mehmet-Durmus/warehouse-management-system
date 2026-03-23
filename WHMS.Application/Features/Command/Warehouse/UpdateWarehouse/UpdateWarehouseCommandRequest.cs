using MediatR;

namespace WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;

public class UpdateWarehouseCommandRequest : IRequest<UpdateWarehouseCommandResponse>
{
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public string? AddressLine { get; set; }
    public string? PostalCode { get; set; }
}