using MediatR;

namespace WHMS.Application.Features.Command.Store.UpdateStore;

public class UpdateStoreCommandRequest : IRequest<UpdateStoreCommandResponse>
{
    public string? StoreId { get; set; }
    public string? StoreName { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public string? AddressLine { get; set; }
    public string? PostalCode { get; set; }
}