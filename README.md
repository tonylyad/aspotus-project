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
- `Aspotus.Orders.Api` - заказы
- `Aspotus.Notifications.Worker` - обработка асинхронных уведомлений
- `Aspotus.Gateway` - gateway, Identity, JWT, роли, Swagger
- `Aspotus.Shared` — общие сущности/контракты, если используются

## Технологии

- .NET 10
- ASP.NET Core Web API
- EF Core
- SQLite
- Redis
- RabbitMQ
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

При первом запуске Docker соберёт образы и запустит шесть контейнеров: Gateway,
Catalog API, Orders API, Notifications Worker, Redis и RabbitMQ. Миграции EF Core
применяются автоматически при старте сервисов.

Проверить состояние контейнеров:

```powershell
docker compose -f src/docker-compose.yml ps
```

После запуска доступны:

- Gateway и общий Swagger: <http://localhost:5230/swagger>
- Catalog API: <http://localhost:5299/swagger>
- Orders API: <http://localhost:5115/swagger>
- Redis: `localhost:6379`
- RabbitMQ Management: <http://localhost:15672>

Данные для входа в RabbitMQ Management в локальном окружении:

- логин: `aspotus`
- пароль: `aspotus-dev`

Внешние запросы к API рекомендуется отправлять через Gateway:

- Catalog: `http://localhost:5230/catalog/...`
- Orders: `http://localhost:5230/orders/...`
- Files: `http://localhost:5230/files/...`
- Admin Web: `http://localhost:5174`

Админка: http://localhost:5174

Тестовые пользователи создаются автоматически (пароль для всех — `123456`):

- `admin` — администратор;
- `operator` — оператор заказов;
- `moderator` — модератор контента;
- `customer` — покупатель.

SQLite-базы хранятся на компьютере вне контейнеров:

- `src/data/catalog/catalog.db`
- `src/data/gateway/gateway.db`
- `src/data/orders/orders.db`
- `src/data/notifications/notifications.db`

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

## Асинхронные уведомления

При создании заказа Orders API сохраняет заказ и событие `OrderCreated` в таблицу
`OutboxMessages` одной транзакцией. Фоновый publisher отправляет событие в RabbitMQ
через exchange `aspotus.events` с routing key `orders.created.v1`.

Notifications Worker получает события из durable-очереди
`notifications.order-created`, логирует уведомление и сохраняет идентификатор
обработанного события в `notifications.db`. Inbox предотвращает повторную обработку
события при повторной доставке RabbitMQ.

Текущий поток:

```text
Gateway -> Orders API -> Orders DB / Outbox -> RabbitMQ -> Notifications Worker
```

## Миграции
dotnet ef database update --project Aspotus.Catalog.Api --startup-project Aspotus.Catalog.Api
dotnet ef database update --project Aspotus.Orders.Api --startup-project Aspotus.Orders.Api
dotnet ef database update --project Aspotus.Gateway --startup-project Aspotus.Gateway
