using System.Collections;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Queries.Location.GetLocationData;

public class GetLocationDataQueryHandler : IRequestHandler<GetLocationDataQueryRequest, GetLocationDataQueryResponse>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationDataQueryHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<GetLocationDataQueryResponse> Handle(GetLocationDataQueryRequest request, CancellationToken cancellationToken)
    {
        var cities = await _locationRepository.GetLocationData();
        GetLocationDataQueryResponse response = new() { Cities = [] };

        foreach (var city in cities)
        {
            GetLocationDataResultCityDto cityDto = new()
            {
                CityId = city.CityId.ToString(),
                CityName = city.Name,
                Districts = []
            };
            response.Cities.Add(cityDto);
            if (city.Districts is not null)
            {
                foreach (var district in city.Districts)
                {
                    GetLocationDataResultDistrictDto districtDto = new()
                    {
                        DistrictId = district.DistrictId.ToString(),
                        DistrictName = district.Name,
                        Neighborhoods = []
                    };
                    cityDto.Districts.Add(districtDto);
                    if (district.Neighborhoods is not null)
                    {
                        foreach (var neighborhood in district.Neighborhoods)
                        {
                            districtDto.Neighborhoods.Add(new()
                            {
                                NeighborhoodId = neighborhood.NeighborhoodId.ToString(),
                                NeighborhoodName = neighborhood.Name
                            });
                        }
                    }
                }
            }
        }
        return response;
    }
}