# aspotus-project agent notes

## Scope and layout
- .NET 10 monorepo in `.slnx` format (`aspotus-project.slnx`), with 4 projects under `src/` and 1 test project under `tests/Unit.Test`.
- Frontend is not in this repo; customer web app lives at `C:\otus\aspotus-web`.
- `tests/Unit.Test` references only `src/Aspotus.Catalog.Api` (no Gateway/Orders tests yet).
- `Aspotus.Shared` has no source files (empty folders `Dtos/`, `Enums/`, `Exceptions/`) — not referenced by any project.

## Commands that matter
- Build all: `dotnet build`
- Run all tests: `dotnet test`
- Run one test class: `dotnet test --filter "FullyQualifiedName~BrandServiceTests"`
- Run services: `dotnet run --project src/Aspotus.Gateway` (5230), `dotnet run --project src/Aspotus.Catalog.Api` (5299), `dotnet run --project src/Aspotus.Orders.Api` (5115)

## Runtime and data gotchas
- All services use SQLite files in project directories: `gateway.db`, `catalog.db`, `orders.db`.
- Gateway and Catalog auto-apply EF migrations at startup; Orders does not.
- Before running Orders against a fresh DB: `dotnet ef database update --project src/Aspotus.Orders.Api --startup-project src/Aspotus.Orders.Api`
- For EF CLI, always pass both `--project` and `--startup-project`.
- JWT config: Issuer=`Aspotus.Gateway`, Audience=`Aspotus.Frontend`, `ExpiresInMinutes=120`, SecretKey in `appsettings.json`.

## Gateway specifics
- YARP strips prefixes: `/catalog/{**catch-all}` -> Catalog API (`http://localhost:5299/`), `/orders/{**catch-all}` -> Orders API (`http://localhost:5115/`).
- Gateway seeds 4 roles (`Customer`, `ContentModerator`, `Operator`, `Admin`) and default admin `admin`/`123456`/`admin@aspotus.com` on startup.
- Identity password policy is intentionally weak: length 6, no complexity requirements.
- Gateway Swagger UI (`/swagger`) includes its own endpoints plus proxied Catalog and Orders OpenAPI docs.
- Gateway passes user context to downstream services via headers: `X-User-Id`, `X-User-Email`, `X-User-Roles` (comma-separated).
- Gateway access rules (enforced in proxy middleware):
  - `GET /catalog` — anonymous
  - `POST/PUT/PATCH/DELETE /catalog` — `ContentModerator`, `Admin`
  - `POST /orders/api/orders/parts`, `/orders/api/orders/cars` — `Customer`, `Operator`, `Admin`
  - `GET /orders/api/orders` — `Operator`, `Admin`
  - `DELETE /orders/api/orders` — `Admin` only

## Catalog and Orders behavior
- Both APIs use custom exception middleware: `NotFoundException`->404, `AlreadyExistsException`->409, `ValidationException`/`InvalidOperationException`->400, else 500.
- Catalog has service/repository layering and normalizes string fields with `.Trim()` before validation/persistence.
- Non-obvious catalog routes: `GET /api/models/by-brand/{brandId:guid}` and `GET /api/generations/by-model/{modelId:guid}`.
- Swagger exposure differs: Catalog Swagger only in Development; Orders Swagger enabled in all environments.
- Catalog seeds 2 brands (Toyota, BMW) + models, generations, cars, categories, manufacturers, parts on first run. Orders Seed folder is empty.

## Test conventions
- Assertion style for new tests is `AwesomeAssertions` (`.Should()`); `FluentAssertions` exists but is legacy (2 files).
- Existing tests use `Bogus` inline `Faker<T>` data setup; `Moq` for mocking; `xUnit` (`[Fact]`). AutoBogus and AutoFixture are also available but not the primary style.
- Test project has `<Using Include="Xunit" />` at project level — no need for `using Xunit;` in test files.
