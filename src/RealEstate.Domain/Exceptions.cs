namespace RealEstate.Domain;

/// <summary>
/// Виняток, який викидається при невалідній адресі.
/// </summary>
public class InvalidAddressException : Exception
{
    public InvalidAddressException() : base("Адреса не може бути порожною.") { }
    public InvalidAddressException(string message) : base(message) { }
}

/// <summary>
/// Виняток, який викидається при невалідній ціні.
/// </summary>
public class InvalidPriceException : Exception
{
    public InvalidPriceException() : base("Ціна повинна бути більше нуля.") { }
    public InvalidPriceException(string message) : base(message) { }
}

/// <summary>
/// Виняток, який викидається коли об'єкт вже проданий.
/// </summary>
public class AlreadySoldException : Exception
{
    public AlreadySoldException() : base("Об'єкт вже проданий і не може бути проданий ще раз.") { }
    public AlreadySoldException(string message) : base(message) { }
}
