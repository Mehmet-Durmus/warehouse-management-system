using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IWasteRecordRepository
{
    Task CreateWasteRecord(WasteRecord wasteRecord);
    Task<List<WasteRecord>> GetWasteRecords(WasteRecordFilter filter, bool applyPagination);
    Task<int> GetWasteRecordCount(WasteRecordFilter filter);
    Task<WasteRecord> GetWasteRecord(Guid wasteRecordId);
    Task Delete(Guid wasteRecordId);
}