namespace WHMS.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task CommitAsync();
}