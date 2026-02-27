using WHMS.Application.Abstractions.Persistence;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly WHMSDbContext _context;

    public UnitOfWork(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}