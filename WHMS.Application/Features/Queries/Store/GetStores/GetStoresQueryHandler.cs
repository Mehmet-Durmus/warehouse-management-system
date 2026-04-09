using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

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
        StoreFilter filter = new StoreFilter
        {
            Name = request.Name,
            CityId = request.CityId,
            DistrictId = request.DistrictId,
            NeighborhoodId = request.NeighborhoodId,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var stores = await _storeRepository.GetStores(filter, applyPagination: true);

        int count = await _storeRepository.GetStoresCount(filter);

        GetStoresQueryResponse response = new() { Stores = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double) count / request.PageSize)
        );
        
        foreach (var store in stores)
            response.Stores.Add(new()
            {
                StoreId = store.Id.ToString(),
                StoreName = store.StoreName,
                CityId = store.Address.CityId.ToString(),
                DistrictId = store.Address.DistrictId.ToString(),
                NeighborhoodId = store.Address.NeighborhoodId.ToString(),
                Address = await _locationRepository.ConvertString(store.Address)                
            });
        return response;
    }
}