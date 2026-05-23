# Developer Guide

## Архітектура
Проєкт побудований за принципами багатошарової архітектури:
1. `Domain` - сутності та бізнес-правила.
2. `Application` - сервіси та логіка Use Cases.
3. `Infrastructure` - збереження даних (JSON).
4. `Console` - рівень представлення.

## Як додати нову нерухомість
Створіть новий клас у `Domain`, що наслідується від `Property`, та оновіть логіку в `PropertyService`.

## Тестування
Запуск тестів: `dotnet test`.
Звіт про покриття: `dotnet test /p:CollectCoverage=true`.