using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;
using WHMS.Domain.ValueObjects;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly WHMSDbContext _whmsContext;

    public LocationRepository(WHMSDbContext whmsContext)
    {
        _whmsContext = whmsContext;
    }

    public async Task<string> ConvertString(Address address)
    {
        string city = await GetCityName(address.CityId);
        string district = await GetDistrictName(address.DistrictId);
        string neighborhood = await GetNeighborhoodName(address.NeighborhoodId);

        return $"{neighborhood} Mah. {address.AddressLine} {district} / {city} {address.PostalCode}";
    }

    public async Task<string> GetCityName(Guid cityId)
    {
        var name = await _whmsContext.Cities
            .Where(c => c.CityId == cityId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync();
        if (name is null)
            throw new Exception("City not found.");
        return name;
    }

    public async Task<string> GetDistrictName(Guid districtId)
    {
        var name = await _whmsContext.Districts
            .Where(d => d.DistrictId == districtId)
            .Select(d => d.Name)
            .FirstOrDefaultAsync();
        if (name is null)
            throw new Exception("District not found.");
        return name;
    }

    public Task<List<string>> GetDistrictsByCity(Guid cityId)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetNeighborhoodName(Guid neighborhoodId)
    {
        var name = await _whmsContext.Neighborhoods
            .Where(n => n.NeighborhoodId == neighborhoodId)
            .Select(n => n.Name)
            .FirstOrDefaultAsync();
        if (name is null)
            throw new Exception("Neighborhood not found.");
        return name;
    }

    public Task<List<string>> GetNeighborhoodsByDistrict(Guid districtId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsAddressValid(Address address)
    {
        return await _whmsContext.Neighborhoods
            .AnyAsync(n => n.NeighborhoodId == address.NeighborhoodId
            && n.DistrictId == address.DistrictId
            && n.District.CityId == address.CityId);
    }
}