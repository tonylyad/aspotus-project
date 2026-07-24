# Documentation

В этой папке находится документация по проекту:
- описание архитектуры
- схемы
- технические решения

Открыть терминал в корне solution, где находится файл .sln, и выполнить:

dotnet ef migrations add InitialCreate --project Aspotus.Catalog.Api --startup-project Aspotus.Catalog.Api
dotnet ef database update --project Aspotus.Catalog.Api --startup-project Aspotus.Catalog.Api

После этого:

будет создан файл базы данных catalog.db
в базе будут созданы таблицы

После этого выбираем Aspotus.Catalog.Api как стартовый проект и запускаем

Для Aspotus.Orders.Api
dotnet ef migrations add InitialCreate --project Aspotus.Orders.Api --startup-project Aspotus.Orders.Api
dotnet ef database update --project Aspotus.Orders.Api --startup-project Aspotus.Orders.Api

Для запуска служб через docker
1) Должны быть создана база orders.db
2) Проверить, установлены ли сертификаты для https командой dotnet dev-certs https --check
   Если команда скажет, что его нет или срок действия истек (обычно выдается на год), сгенерировать и доверить:
   dotnet dev-certs https --trust
3) Выполняем из папки src команду docker-compose up