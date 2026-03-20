using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Persistence.Repositories;

public class WasteRecordRepository : IWasteRecordRepository
{
    public Task CreateWasteRecord(WasteRecord wasteRecord)
    {
        throw new NotImplementedException();
    }

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