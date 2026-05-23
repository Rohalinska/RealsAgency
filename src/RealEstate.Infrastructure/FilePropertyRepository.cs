using System.Text.Json;
using RealEstate.Domain;

namespace RealEstate.Infrastructure;

/// <summary>
/// Реалізація сховища даних на основі JSON-файлу.
/// </summary>
public class FilePropertyRepository : IPropertyRepository
{
    private readonly string _filePath = "database.json";

    public async Task<List<Property>> GetAllAsync()
    {
        if (!File.Exists(_filePath)) return new List<Property>();
        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<Property>>(json) ?? new List<Property>();
    }

    public async Task AddAsync(Property property)
    {
        var properties = await GetAllAsync();
        properties.Add(property);
        var json = JsonSerializer.Serialize(properties, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task UpdateAsync(Property property)
    {
        var properties = await GetAllAsync();
        var index = properties.FindIndex(p => p.Id == property.Id);
        if (index != -1)
        {
            properties[index] = property;
            await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(properties));
        }
    }
}