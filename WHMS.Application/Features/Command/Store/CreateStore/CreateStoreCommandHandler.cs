using System.Security.Cryptography.X509Certificates;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.ValueObjects;
using WHMS.Domain.Entities;
using System.Globalization;

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
        Address address = new(
            request.CityId!,
            request.DistrictId!,
            request.NeighborhoodId!,
            request.PostalCode!,
            request.AddressLine!
        );

        if (!await _locationRepository.IsAddressValid(address))
            throw new Exception("Address data is invalid.");

        string normalizedName = request.StoreName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        if (await _storeRepository.StoreNameExists(normalizedName))
            throw new Exception("This name is being used for another store.");

        Domain.Entities.Store  store = new()
        {
            StoreName = request.StoreName,
            NormalizedName = normalizedName,
            Address = address
        };

        await _storeRepository.CreateStore(store);
        await _unitOfWork.CommitAsync();

        return new()
        {
            StoreId = store.Id.ToString(),
            StoreName = store.StoreName,
            Address = await _locationRepository.ConvertString(store.Address)
        };
    }
}