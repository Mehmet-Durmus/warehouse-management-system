namespace WHMS.Application.Abstractions.Infrastructure;

public interface IJsonDataLoader
{
    T Load<T>(string filePath);
}