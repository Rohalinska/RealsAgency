using RealEstate.Domain;
using RealEstate.Application;
using RealEstate.Infrastructure;

var repo = new FilePropertyRepository();
var service = new PropertyService(repo);

while (true)
{
    Console.WriteLine("REAL ESTATE AGENCY (LAB 35)");
    Console.WriteLine("1. Додати нову квартиру");
    Console.WriteLine("2. Показати всі об'єкти (з JSON-файлу)");
    Console.WriteLine("3. Аналітика: Загальна вартість активів");
    Console.WriteLine("4. Пошук: Найдешевші варіанти");
    Console.WriteLine("5. Статистика за типами");
    Console.WriteLine("0. Вихід");
    Console.Write("Ваш вибір: ");

    var input = Console.ReadLine();
    if (input == "0") break;

    switch (input)
    {
        case "1":
            Console.Write("Адреса: "); string addr = Console.ReadLine() ?? "Unknown";
            Console.Write("Ціна: "); decimal price = decimal.Parse(Console.ReadLine() ?? "0");
            Console.Write("Поверх: "); int floor = int.Parse(Console.ReadLine() ?? "1");
            
            await service.AddPropertyAsync(new Apartment(addr, price, floor));
            Console.WriteLine(" Об'єкт збережено у database.json!");
            break;

        case "2":
            var all = await service.GetAllPropertiesAsync();
            all.ForEach(p => Console.WriteLine($"[{p.Status}] {p.Address}: {p.Price} грн"));
            break;

        case "3":
            var total = await service.GetTotalPortfolioValueAsync();
            Console.WriteLine($" Загальна ціна всіх об'єктів: {total} грн");
            break;

        case "4":
            Console.Write("Введіть максимальний бюджет: ");
            decimal budget = decimal.Parse(Console.ReadLine() ?? "0");
            var cheap = await service.GetCheapestAsync(budget);
            Console.WriteLine($"--- Знайдено {cheap.Count} варіантів ---");
            cheap.ForEach(p => Console.WriteLine($"{p.Address} - {p.Price} грн"));
            break;

        case "5":
            var stats = await service.GetCountByTypeAsync();
            foreach (var s in stats) Console.WriteLine($"{s.Key}: {s.Value} шт.");
            break;

        default:
            Console.WriteLine(" Невірний вибір.");
            break;
    }
}