using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Implementations;
using AwesomeAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Unit.Test.Catalog;

public sealed class InventoryReservationServiceTests
{
    [Fact]
    public async Task Car_CannotBeReservedByTwoOrders_AndBecomesAvailableAfterRelease()
    {
        await using var fixture = await CatalogFixture.CreateAsync();
        var service = new InventoryReservationService(fixture.Context);
        var firstOrderId = Guid.NewGuid();

        await service.ReserveAsync(Request(firstOrderId, "Car", fixture.CarId, 1));

        var secondAttempt = () => service.ReserveAsync(Request(Guid.NewGuid(), "Car", fixture.CarId, 1));
        await secondAttempt.Should().ThrowAsync<AlreadyExistsException>();

        await service.ReleaseAsync(firstOrderId);
        var reservation = await service.ReserveAsync(Request(Guid.NewGuid(), "Car", fixture.CarId, 1));
        reservation.Items.Should().ContainSingle().Which.ProductId.Should().Be(fixture.CarId);
    }

    [Fact]
    public async Task Part_ReservationUsesAvailableStock_NotRawStock()
    {
        await using var fixture = await CatalogFixture.CreateAsync();
        var service = new InventoryReservationService(fixture.Context);

        await service.ReserveAsync(Request(Guid.NewGuid(), "Part", fixture.PartId, 4));
        var secondAttempt = () => service.ReserveAsync(Request(Guid.NewGuid(), "Part", fixture.PartId, 2));

        await secondAttempt.Should().ThrowAsync<AlreadyExistsException>();
        (await service.GetReservedPartQuantitiesAsync())[fixture.PartId].Should().Be(4);
    }

    private static ReserveInventoryRequest Request(Guid orderId, string type, Guid productId, int quantity) => new()
    {
        OrderId = orderId,
        Items = [new ReserveInventoryItemRequest { ProductType = type, ProductId = productId, Quantity = quantity }]
    };

    private sealed class CatalogFixture : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        public CatalogDbContext Context { get; }
        public Guid CarId { get; } = Guid.NewGuid();
        public Guid PartId { get; } = Guid.NewGuid();

        private CatalogFixture(SqliteConnection connection, CatalogDbContext context)
        {
            _connection = connection;
            Context = context;
        }

        public static async Task<CatalogFixture> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var context = new CatalogDbContext(new DbContextOptionsBuilder<CatalogDbContext>().UseSqlite(connection).Options);
            await context.Database.EnsureCreatedAsync();
            var fixture = new CatalogFixture(connection, context);

            var brand = new CarBrand { Id = Guid.NewGuid(), Name = "Test" };
            var model = new CarModel { Id = Guid.NewGuid(), Name = "Model", BrandId = brand.Id };
            var generation = new CarGeneration { Id = Guid.NewGuid(), Name = "I", ModelId = model.Id, YearFrom = 2020 };
            context.Cars.Add(new Car
            {
                Id = fixture.CarId, BrandId = brand.Id, Brand = brand, ModelId = model.Id, Model = model,
                GenerationId = generation.Id, Generation = generation, Year = 2024, Mileage = 1, Price = 1_000_000,
                BodyType = "Sedan", EngineVolume = 2, FuelType = "Petrol", TransmissionType = "Automatic", DriveType = "Fwd"
            });
            var category = new PartCategory { Id = Guid.NewGuid(), Name = "Test" };
            var manufacturer = new PartManufacturer { Id = Guid.NewGuid(), Name = "Test" };
            context.Parts.Add(new Part
            {
                Id = fixture.PartId, Name = "Part", Article = "P-1", Price = 100, StockQuantity = 5,
                CategoryId = category.Id, Category = category, ManufacturerId = manufacturer.Id, Manufacturer = manufacturer
            });
            await context.SaveChangesAsync();
            return fixture;
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
