using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Store;
using WHMS.Application.Features.Queries.Store.GetStore;

namespace WHMS.Application.Features.Queries.Store.GetSores;

public class GetStoresQueryHandler : IRequestHandler<GetStoresQueryRequest, GetStoresQueryResponse>
{
    private readonly IStoreRepository _storeRepository;
    private readonly ILocationRepository _locationRepository;

    public GetStoresQueryHandler(IStoreRepository storeRepository, ILocationRepository locationRepository = null)
    {
        _storeRepository = storeRepository;
        _locationRepository = locationRepository;
    }

    public async Task<GetStoresQueryResponse> Handle(GetStoresQueryRequest request, CancellationToken cancellationToken)
    {
        var stores = await _storeRepository.GetStores();

        GetStoresQueryResponse response = new() {Stores = []};
        foreach (var store in stores)
            response.Stores.Add(new StoreDto
            {
                StoreName = store.StoreName,
                City = await _locationRepository.GetCityName(store.Address.CityId),
                District = await _locationRepository.GetDistrictName(store.Address.DistrictId),
                Neighborhood = await _locationRepository.GetNeighborhoodName(store.Address.NeighborhoodId),
                AddressLine = store.Address.AddressLine,
                PostalCode = store.Address.PostalCode,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt
            });
        return response;
    }
}