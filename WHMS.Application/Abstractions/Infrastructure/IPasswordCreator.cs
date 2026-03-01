namespace WHMS.Application.Abstractions.Infrastructure;

public interface IPasswordCreator
{
    Task<string> CreateTempPassword();
}