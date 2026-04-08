using System.Globalization;
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

        Address address = new(
            Guid.Parse(request.CityId!),
            Guid.Parse(request.DistrictId!),
            Guid.Parse(request.NeighborhoodId!),
            request.PostalCode!,
            request.AddressLine!
        );

        if (!await _locationRepository.IsAddressValid(address))
            throw new Exception("Address data is invalid.");

        string normalizedName = request.StoreName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        bool isNameUsed = await _storeRepository.StoreNameExists(normalizedName);
        if (!store.NormalizedName.Equals(normalizedName) && isNameUsed)
            throw new Exception("This name is being used for another store");

        store.StoreName = request.StoreName;
        store.NormalizedName = normalizedName;
        store.Address = address;

        await _unitOfWork.CommitAsync();

        return new()
        {
            StoreName = store.StoreName,
            Address = await _locationRepository.ConvertString(store.Address)
        };
    }
}