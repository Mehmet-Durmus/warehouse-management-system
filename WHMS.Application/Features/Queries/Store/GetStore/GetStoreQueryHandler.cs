using System.Runtime.Serialization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Store.GetStore;

public class GetStoreQueryHandler : IRequestHandler<GetStoreQueryRequest, GetStoreQueryResponse>
{
    private readonly IStoreRepository _storeRepository;
    private readonly ILocationRepository _locationRepository;

    public GetStoreQueryHandler(IStoreRepository storeRepository, ILocationRepository locationRepository)
    {
        _storeRepository = storeRepository;
        _locationRepository = locationRepository;
    }

    public async Task<GetStoreQueryResponse> Handle(GetStoreQueryRequest request, CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetStore(Guid.Parse(request.StoreId!));
        if (store is null)
            throw new Exception("Store not found.");

        return new()
        {
            StoreName = store.StoreName,
            CityId = store.Address.CityId.ToString(),
            DistrictId = store.Address.DistrictId.ToString(),
            NeighborhoodId = store.Address.NeighborhoodId.ToString(),
            Address = await _locationRepository.ConvertString(store.Address)
        };
    }
}