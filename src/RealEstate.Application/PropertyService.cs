using RealEstate.Domain;

namespace RealEstate.Application;

/// <summary>
/// Сервіс для керування бізнес-логікою нерухомості.
/// </summary>
public class PropertyService
{
    private readonly IPropertyRepository _repo;

    public PropertyService(IPropertyRepository repo) => _repo = repo;

    /// <summary> Додає новий об'єкт у базу. </summary>
    public async Task AddPropertyAsync(Property property) => await _repo.AddAsync(property);

    /// <summary> Отримує всі об'єкти. </summary>
    public async Task<List<Property>> GetAllPropertiesAsync() => await _repo.GetAllAsync();

    /// <summary> Рахує загальну вартість усієї нерухомості (LINQ). </summary>
    public async Task<decimal> GetTotalPortfolioValueAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.Sum(p => p.Price);
    }

    /// <summary> Групує об'єкти за типами для звіту (LINQ). </summary>
    public async Task<Dictionary<string, int>> GetCountByTypeAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.GroupBy(p => p.GetType().Name)
                  .ToDictionary(g => g.Key, g => g.Count());
    }
}