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