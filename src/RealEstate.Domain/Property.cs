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

    protected Property() { }

    protected Property(string address, decimal price)
    {
        if (string.IsNullOrWhiteSpace(address)) throw new InvalidAddressException();
        if (price <= 0) throw new InvalidPriceException();
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
}

/// <summary>
/// Представляє житлову квартиру.
/// </summary>
public class Apartment : Property
{
    public int Floor { get; set; }
    public Apartment() { }
    public Apartment(string a, decimal p, int f) : base(a, p) => Floor = f;
}