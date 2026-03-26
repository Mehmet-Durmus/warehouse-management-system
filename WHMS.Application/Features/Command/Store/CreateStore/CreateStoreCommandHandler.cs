using System.Security.Cryptography.X509Certificates;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.ValueObjects;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Store.CreateStore;

public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommandRequest, CreateStoreCommandResponse>
{
    private readonly IStoreRepository _storeRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStoreCommandHandler(IStoreRepository storeRepository, IUnitOfWork unitOfWork, ILocationRepository locationRepository)
    {
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
        _locationRepository = locationRepository;
    }

    public async Task<CreateStoreCommandResponse> Handle(CreateStoreCommandRequest request, CancellationToken cancellationToken)
    {
        // bool isAddressValid = await _locationRepository.IsAddressValid(
        //     Guid.Parse(request.CityId!),
        //     Guid.Parse(request.DistrictId!),
        //     Guid.Parse(request.NeighborhoodId!));
        // if (!isAddressValid)
        //     throw new Exception("Address data is invalid.");

        // Address address = new()
        // {
        //     CityId = Guid.Parse(request.CityId!),
        //     DistrictId = Guid.Parse(request.DistrictId!),
        //     NeighborhoodId = Guid.Parse(request.NeighborhoodId!),
        //     AddressLine = request.AddressLine!,
        //     PostalCode = request.PostalCode!
        // };

        // WHMS.Domain.Entities.Store store = new() { StoreName = request.StoreName!, Address = address};

        // await _storeRepository.CreateStore(store);
        // await _unitOfWork.CommitAsync();

        return new();
    }
}