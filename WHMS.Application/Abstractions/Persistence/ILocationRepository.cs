using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Abstractions.Persistence;

public interface ILocationRepository
{
    Task<string> GetCityName(Guid cityId);
    Task<string> GetDistrictName(Guid districtId);
    Task<List<string>> GetDistrictsByCity(Guid cityId);
    Task<bool> IsAddressValid(Address address);
    Task<string> GetNeighborhoodName(Guid neighborhoodId);
    Task<List<string>> GetNeighborhoodsByDistrict(Guid districtId);
    Task<string> ConvertString(Address address);
}