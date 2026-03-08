using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Store;
using WHMS.Application.Features.Queries.Store.GetStores;
using WHMS.Application.Filters;

namespace WHMS.Application.Features.Queries.Store.GetStores;

public class GetStoresQueryHandler : IRequestHandler<GetStoresQueryRequest, GetStoresQueryResponse>
{
    private readonly IStoreRepository _storeRepository;
    private readonly ILocationRepository _locationRepository;

    public GetStoresQueryHandler(IStoreRepository storeRepository, ILocationRepository locationRepository)
    {
        _storeRepository = storeRepository;
        _locationRepository = locationRepository;
    }

    public async Task<GetStoresQueryResponse> Handle(GetStoresQueryRequest request, CancellationToken cancellationToken)
    {

        var stores = await _storeRepository.GetStores(
            new StoreFilter
            {
                CityId = request.CityId,
                DistrictId = request.DistrictId,
                Page = request.Page,
                PageSize = request.PageSize
            }
        );

        int count = await _storeRepository.GetStoresCount(
            new StoreFilter
            {
                CityId = request.CityId,
                DistrictId = request.DistrictId
            }
        );

        GetStoresQueryResponse response = new() 
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)count / request.PageSize),
            Stores = []
        };
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