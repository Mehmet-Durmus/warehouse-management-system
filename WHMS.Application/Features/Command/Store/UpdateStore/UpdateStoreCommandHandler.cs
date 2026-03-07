using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Features.Command.Store.UpdateStore;

public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommandRequest, UpdateStoreCommandResponse>
{
    private readonly IStoreRepository _storeRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStoreCommandHandler(IStoreRepository storeRepository, ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _storeRepository = storeRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateStoreCommandResponse> Handle(UpdateStoreCommandRequest request, CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetStore(Guid.Parse(request.StoreId!));
        if (store is null)
            throw new Exception("Store not found.");
        
        bool isAddressValid = await _locationRepository.IsAddressValid(
            Guid.Parse(request.CityId!),
            Guid.Parse(request.DistrictId!),
            Guid.Parse(request.NeighborhoodId!));
        if (!isAddressValid)
            throw new Exception("Address data is invalid.");

        store.StoreName = request.StoreName!;
        store.Address = new Address
        {
            CityId = Guid.Parse(request.CityId!),
            DistrictId = Guid.Parse(request.DistrictId!),
            NeighborhoodId = Guid.Parse(request.NeighborhoodId!),
            AddressLine = request.AddressLine!,
            PostalCode = request.PostalCode!
        };
        _storeRepository.Update(store);
        await _unitOfWork.CommitAsync();
        return new();
    }
}