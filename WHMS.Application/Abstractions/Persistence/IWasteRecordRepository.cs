using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IWasteRecordRepository
{
    Task CreateWasteRecord(WasteRecord wasteRecord);
    Task<List<WasteRecord>> GetWasteRecords();
    Task<int> GetWasteRecordCount();
    Task<WasteRecord> GetWasteRecord(Guid wasteRecordId);
    void Delete(WasteRecord wasteRecord);
}