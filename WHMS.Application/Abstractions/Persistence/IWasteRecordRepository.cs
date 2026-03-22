using WHMS.Application.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IWasteRecordRepository
{
    Task CreateWasteRecord(WasteRecord wasteRecord);
    Task<List<WasteRecord>> GetWasteRecords(WasteRecordFilter filter, bool withPagination);
    Task<int> GetWasteRecordCount(WasteRecordFilter filter);
    Task<WasteRecord> GetWasteRecord(Guid wasteRecordId);
    void Delete(WasteRecord wasteRecord);
}