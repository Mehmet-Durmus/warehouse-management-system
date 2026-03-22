using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Extensions;
using WHMS.Application.Filters;
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

    public async Task Delete(Guid wasteRecordId)
    {
        var wasteRecord = await GetWasteRecord(wasteRecordId);
        wasteRecord.IsActive = false;
    }

    public async Task<WasteRecord> GetWasteRecord(Guid wasteRecordId)
    {
        var wasteRecord = await _context.WasteRecords.FindAsync(wasteRecordId);
        return wasteRecord!;
    }

    public async Task<int> GetWasteRecordCount(WasteRecordFilter filter)
        => await _context.WasteRecords
            .ApplyWasteRecordFilter(filter, withPagination: false)
            .CountAsync();

    public async Task<List<WasteRecord>> GetWasteRecords(WasteRecordFilter filter, bool withPagination)
        => await _context.WasteRecords
            .ApplyWasteRecordFilter(filter)
            .ToListAsync();
}