namespace WHMS.Application.Abstractions.Persistence;

public interface ILocationRepository
{
    Task<string> GetCityName(Guid cityId);
    Task<string> GetDistrictName(Guid districtId);
    Task<List<string>> GetDistrictsByCity(Guid cityId);
    Task<string> GetNeighborhoodName(Guid neighborhoodId);
    Task<List<string>> GetNeighborhoodsByDistrict(Guid districtId);
}