# aspotus-project
Общий проект для курса Otus C# ASP.NET Core разработчик.

## Разработчики:
- Антон Лядов
- Максим Сафронов
- Алексей Силаев
- Шадчнев Дмитрий
- Карлштейн Елена

## Состав solution

- `Aspotus.Catalog.Api` — каталог
- `Aspotus.Orders.Api` — заказы
- `Aspotus.Gateway` — gateway, Identity, JWT, роли, Swagger
- `Aspotus.Shared` — общие сущности/контракты, если используются

## Технологии

- .NET 10
- ASP.NET Core Web API
- EF Core
- SQLite
- Swagger
- XML-аннотации для Swagger
- YARP Reverse Proxy
- ASP.NET Core Identity
- JWT Bearer Authentication

---

## Миграции
dotnet ef database update --project Aspotus.Catalog.Api --startup-project Aspotus.Catalog.Api
dotnet ef database update --project Aspotus.Orders.Api --startup-project Aspotus.Orders.Api
dotnet ef database update --project Aspotus.Gateway --startup-project Aspotus.Gateway