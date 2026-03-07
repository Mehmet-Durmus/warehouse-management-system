using MediatR;

namespace WHMS.Application.Features.Command.Store.CreateStore;

public class CreateStoreCommandRequest : IRequest<CreateStoreCommandResponse>
{
    public string? StoreName { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public string? AddressLine { get; set; }
    public string? PostalCode { get; set; }
}