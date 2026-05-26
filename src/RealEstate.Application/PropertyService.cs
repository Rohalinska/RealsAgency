using RealEstate.Domain;
using RealEstate.Infrastructure;

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
    
    /// <summary> Знаходить об'єкти з ціною до максимального бюджету. </summary>
    public async Task<List<Property>> GetCheapestAsync(decimal maxBudget)
    {
        var all = await _repo.GetAllAsync();
        return all.Where(p => p.Price <= maxBudget).OrderBy(p => p.Price).ToList();
    }
    
    /// <summary> Рахує кількість проданих об'єктів. </summary>
    public async Task<int> GetSoldPropertiesCountAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.Count(p => p.IsSold);
    }
    
    /// <summary> Отримує список доступних (не проданих) об'єктів. </summary>
    public async Task<List<Property>> GetAvailablePropertiesAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.Where(p => !p.IsSold).ToList();
    }
    
    /// <summary> Розраховує загальну очікувану комісію. </summary>
    public async Task<decimal> GetTotalExpectedCommissionAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.Sum(p => p.CalculateCommission());
    }
    
    /// <summary> Пошук об'єкта за адресою. </summary>
    public async Task<Property?> FindPropertyByAddressAsync(string address)
    {
        var all = await _repo.GetAllAsync();
        return all.FirstOrDefault(p => p.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary> Отримує найдешевший об'єкт. </summary>
    public async Task<Property?> GetCheapestPropertyAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.OrderBy(p => p.Price).FirstOrDefault();
    }
    
    /// <summary> Отримує найдорожчий об'єкт. </summary>
    public async Task<Property?> GetMostExpensivePropertyAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(p => p.Price).FirstOrDefault();
    }
    
    /// <summary> Видаляє об'єкт з базу. </summary>
    public async Task DeletePropertyAsync(int id)
    {
        await _repo.DeleteAsync(id);
    }
    
    /// <summary> Оновлює об'єкт у базі. </summary>
    public async Task UpdatePropertyAsync(Property property)
    {
        await _repo.UpdateAsync(property);
    }
    
    /// <summary> Отримує квартири за кількістю кімнат (поверхів). </summary>
    public async Task<List<Property>> GetPropertiesByRoomsAsync(int roomCount)
    {
        var all = await _repo.GetAllAsync();
        return all.OfType<Apartment>()
                  .Where(a => a.Floor == roomCount)
                  .Cast<Property>()
                  .ToList();
    }
}