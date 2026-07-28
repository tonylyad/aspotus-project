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
- Redis
- Swagger
- XML-аннотации для Swagger
- YARP Reverse Proxy
- ASP.NET Core Identity
- JWT Bearer Authentication

---

## Запуск в Docker

### Требования

- установлен и запущен Docker Desktop;
- Docker работает в режиме Linux containers.

### Запуск

Выполните из корневого каталога репозитория:

```powershell
docker compose -f src/docker-compose.yml up --build -d
```

При первом запуске Docker соберёт образы и запустит четыре контейнера: Gateway,
Catalog API, Orders API и Redis. Миграции EF Core применяются автоматически при
старте сервисов.

Проверить состояние контейнеров:

```powershell
docker compose -f src/docker-compose.yml ps
```

После запуска доступны:

- Gateway и общий Swagger: <http://localhost:5230/swagger>
- Catalog API: <http://localhost:5299/swagger>
- Orders API: <http://localhost:5115/swagger>
- Redis: `localhost:6379`

Внешние запросы к API рекомендуется отправлять через Gateway:

- Catalog: `http://localhost:5230/catalog/...`
- Orders: `http://localhost:5230/orders/...`

SQLite-базы хранятся на компьютере вне контейнеров:

- `src/data/catalog/catalog.db`
- `src/data/gateway/gateway.db`
- `src/data/orders/orders.db`

Поэтому данные сохраняются после остановки и пересоздания контейнеров.

Посмотреть логи всех сервисов:

```powershell
docker compose -f src/docker-compose.yml logs -f
```

Пересобрать и перезапустить сервисы после изменения исходного кода:

```powershell
docker compose -f src/docker-compose.yml up --build -d
```

Остановить и удалить контейнеры и сеть проекта:

```powershell
docker compose -f src/docker-compose.yml down
```

Команда `down` не удаляет SQLite-файлы из `src/data`.

## Кэширование

Catalog API использует Redis как распределённый кэш для списка марок и отдельных
марок автомобилей. Время хранения по умолчанию — 10 минут, его можно изменить
параметром `Cache:BrandsExpirationMinutes`.

При создании, изменении или удалении марки соответствующие ключи удаляются из
Redis. Если Redis временно недоступен, Catalog API продолжает работать с SQLite.

## Миграции
dotnet ef database update --project Aspotus.Catalog.Api --startup-project Aspotus.Catalog.Api
dotnet ef database update --project Aspotus.Orders.Api --startup-project Aspotus.Orders.Api
dotnet ef database update --project Aspotus.Gateway --startup-project Aspotus.Gateway
