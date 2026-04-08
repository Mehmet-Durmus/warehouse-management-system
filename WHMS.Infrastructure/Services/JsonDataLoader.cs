using System.Text;
using System.Text.Json;
using WHMS.Application.Abstractions.Infrastructure;

namespace WHMS.Infrastructure.Services;

public class JsonDataLoader : IJsonDataLoader
{
    public T Load<T>(string filePath)
{
    if (!File.Exists(filePath))
        throw new FileNotFoundException($"Dosya bulunamadı: {filePath}");

    var json = File.ReadAllText(filePath, Encoding.UTF8);
    
    return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })
        ?? throw new InvalidOperationException($"Dosya deserialize edilemedi: {filePath}");
}
}