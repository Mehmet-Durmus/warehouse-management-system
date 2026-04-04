using WHMS.Domain.Entities;
using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Abstractions.Persistence;

public interface ILocationRepository
{
    Task<string> GetCityName(Guid cityId);
    Task<string> GetDistrictName(Guid districtId);
    Task<bool> IsAddressValid(Address address);
    Task<string> GetNeighborhoodName(Guid neighborhoodId);
    Task<string> ConvertString(Address address);
    Task<List<City>> GetLocationData();
}