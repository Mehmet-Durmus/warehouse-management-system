using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.Repositories;

public class WasteRecordRepository : IWasteRecordRepository
{
    private readonly WHMSDbContext _context;

    public WasteRecordRepository(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task CreateWasteRecord(WasteRecord wasteRecord)
        => await _context.WasteRecords.AddAsync(wasteRecord);

    public void Delete(WasteRecord wasteRecord)
    {
        throw new NotImplementedException();
    }

    public Task<WasteRecord> GetWasteRecord(Guid wasteRecordId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetWasteRecordCount()
    {
        throw new NotImplementedException();
    }

    public Task<List<WasteRecord>> GetWasteRecords()
    {
        throw new NotImplementedException();
    }
}