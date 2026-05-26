using RealEstate.Domain;

namespace RealEstate.Infrastructure;

/// <summary>
/// Інтерфейс для доступу до даних про нерухомість.
/// </summary>
public interface IPropertyRepository
{
    /// <summary> Отримує всі об'єкти нерухомості. </summary>
    Task<List<Property>> GetAllAsync();

    /// <summary> Додає новий об'єкт нерухомості. </summary>
    Task AddAsync(Property property);

    /// <summary> Оновлює існуючий об'єкт нерухомості. </summary>
    Task UpdateAsync(Property property);
    
    /// <summary> Видаляє об'єкт нерухомості по Id. </summary>
    Task DeleteAsync(int id);
}
