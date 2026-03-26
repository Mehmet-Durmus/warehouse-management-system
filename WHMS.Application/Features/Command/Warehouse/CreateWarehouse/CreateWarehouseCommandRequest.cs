using MediatR;

namespace WHMS.Application.Features.Command.Warehouse.CreateWarehouse;

public class CreateWarehouseCommandRequest : IRequest<CreateWarehouseCommandResponse>
{
    public string? WarehouseName { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public string? AddressLine { get; set; }
    public string? PostalCode { get; set; }
    public string? ManagerId { get; set; }
    public List<string>? StaffIds { get; set; }
}