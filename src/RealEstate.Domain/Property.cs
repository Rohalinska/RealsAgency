using System;

namespace RealEstate.Domain;

/// <summary>
/// Базовий клас для всіх об'єктів нерухомості.
/// </summary>
public abstract class Property
{
    /// <summary> Унікальний ідентифікатор. </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary> Повна адреса об'єкта. </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary> Вартість об'єкта в гривнях. </summary>
    public decimal Price { get; set; }
    
    /// <summary> Поточний статус (Доступно/Продано). </summary>
    public PropertyStatus Status { get; set; } = PropertyStatus.Available;
    
    /// <summary> Чи проданий об'єкт. </summary>
    public bool IsSold => Status == PropertyStatus.Sold;

    protected Property() { }

    protected Property(string address, decimal price)
    {
        if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Адреса не може бути порожною.", nameof(address));
        if (price < 0) throw new ArgumentException("Ціна не може бути від'ємною.", nameof(price));
        Address = address;
        Price = price;
    }

    /// <summary>
    /// Переводить об'єкт у статус "Продано".
    /// </summary>
    /// <exception cref="AlreadySoldException">Виникає, якщо об'єкт уже проданий.</exception>
    public void MarkAsSold()
    {
        if (Status == PropertyStatus.Sold) throw new AlreadySoldException();
        Status = PropertyStatus.Sold;
    }
    
    /// <summary>
    /// Оновлює ціну об'єкта.
    /// </summary>
    /// <exception cref="ArgumentException">Виникає, якщо ціна невалідна.</exception>
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Ціна не може бути від'ємною.", nameof(newPrice));
        Price = newPrice;
    }
    
    /// <summary>
    /// Розраховує комісію агентства.
    /// </summary>
    public abstract decimal CalculateCommission();
}

/// <summary>
/// Представляє житлову квартиру.
/// </summary>
public class Apartment : Property
{
    public int Floor { get; set; }
    
    public Apartment() { }
    
    public Apartment(string a, decimal p, int f) : base(a, p)
    {
        if (f <= 0) throw new ArgumentException("Поверх повинен бути більше 0.", nameof(f));
        Floor = f;
    }
    
    /// <summary>
    /// Розраховує комісію агентства (5% від вартості квартири).
    /// </summary>
    /// <returns>Розмір комісії.</returns>
    public override decimal CalculateCommission()
    {
        return Price * 0.05m;
    }
}

/// <summary>
/// Представляє житловий будинок.
/// </summary>
public class House : Property
{
    public int Floors { get; set; }
    
    public House() { }
    
    public House(string a, decimal p, int floors) : base(a, p)
    {
        if (floors <= 0) throw new ArgumentException("Кількість поверхів повинна бути більше 0.", nameof(floors));
        Floors = floors;
    }
    
    /// <summary>
    /// Розраховує комісію агентства (7% від вартості будинку).
    /// </summary>
    /// <returns>Розмір комісії.</returns>
    public override decimal CalculateCommission()
    {
        return Price * 0.07m;
    }
}