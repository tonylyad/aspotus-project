using System.Data;
using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Services.Implementations;

public class InventoryReservationService : IInventoryReservationService
{
    public const string CarProductType = "Car";
    public const string PartProductType = "Part";

    private readonly CatalogDbContext _context;

    public InventoryReservationService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryReservationResponse> ReserveAsync(
        ReserveInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedItems = request.Items
            .GroupBy(x => new { Type = NormalizeType(x.ProductType), x.ProductId })
            .Select(group => new ReserveInventoryItemRequest
            {
                ProductType = group.Key.Type,
                ProductId = group.Key.ProductId,
                Quantity = group.Key.Type == CarProductType ? 1 : group.Sum(x => x.Quantity)
            })
            .ToList();

        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var existing = await _context.InventoryReservations
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken);

        if (existing is not null)
        {
            await transaction.CommitAsync(cancellationToken);
            return Map(existing);
        }

        var reservation = new InventoryReservation
        {
            OrderId = request.OrderId,
            UserId = request.UserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Ранняя запись сериализует конкурирующие резервы в SQLite.
        await _context.InventoryReservations.AddAsync(reservation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var item in normalizedItems)
        {
            var reservationItem = item.ProductType == CarProductType
                ? await ReserveCarAsync(request.OrderId, item.ProductId, cancellationToken)
                : await ReservePartAsync(request.OrderId, item.ProductId, item.Quantity, cancellationToken);

            reservationItem.Reservation = reservation;
            reservation.Items.Add(reservationItem);
            await _context.InventoryReservationItems.AddAsync(reservationItem, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Map(reservation);
    }

    public async Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.InventoryReservations
            .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);

        if (reservation is null)
        {
            return;
        }

        _context.InventoryReservations.Remove(reservation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<HashSet<Guid>> GetReservedCarIdsAsync(CancellationToken cancellationToken = default)
    {
        var ids = await _context.InventoryReservationItems
            .AsNoTracking()
            .Where(x => x.ProductType == CarProductType)
            .Select(x => x.ProductId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    public async Task<Dictionary<Guid, int>> GetReservedPartQuantitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryReservationItems
            .AsNoTracking()
            .Where(x => x.ProductType == PartProductType)
            .GroupBy(x => x.ProductId)
            .ToDictionaryAsync(x => x.Key, x => x.Sum(item => item.Quantity), cancellationToken);
    }

    private async Task<InventoryReservationItem> ReserveCarAsync(
        Guid orderId,
        Guid carId,
        CancellationToken cancellationToken)
    {
        var car = await _context.Cars
            .AsNoTracking()
            .Include(x => x.Brand)
            .Include(x => x.Model)
            .Include(x => x.Generation)
            .FirstOrDefaultAsync(x => x.Id == carId, cancellationToken)
            ?? throw new NotFoundException("Автомобиль не найден.");

        var isReserved = await _context.InventoryReservationItems
            .AnyAsync(x => x.ProductType == CarProductType && x.ProductId == carId, cancellationToken);

        if (isReserved)
        {
            throw new AlreadyExistsException("Автомобиль уже находится в другом заказе.");
        }

        if (car.Price <= 0)
        {
            throw new ValidationException("Для автомобиля не указана корректная цена.");
        }

        return new InventoryReservationItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductType = CarProductType,
            ProductId = car.Id,
            Quantity = 1,
            UnitPrice = car.Price,
            Name = $"{car.Brand.Name} {car.Model.Name}",
            BrandName = car.Brand.Name,
            ModelName = car.Model.Name,
            GenerationName = car.Generation.Name,
            Year = car.Year
        };
    }

    private async Task<InventoryReservationItem> ReservePartAsync(
        Guid orderId,
        Guid partId,
        int quantity,
        CancellationToken cancellationToken)
    {
        var part = await _context.Parts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == partId, cancellationToken)
            ?? throw new NotFoundException("Запчасть не найдена.");

        var reserved = await _context.InventoryReservationItems
            .Where(x => x.ProductType == PartProductType && x.ProductId == partId)
            .SumAsync(x => (int?)x.Quantity, cancellationToken) ?? 0;

        if (part.StockQuantity - reserved < quantity)
        {
            throw new AlreadyExistsException($"Недостаточно доступного количества запчасти «{part.Name}».");
        }

        return new InventoryReservationItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductType = PartProductType,
            ProductId = part.Id,
            Quantity = quantity,
            UnitPrice = part.Price,
            Name = part.Name,
            Article = part.Article
        };
    }

    private static string NormalizeType(string productType) =>
        productType.Equals(CarProductType, StringComparison.OrdinalIgnoreCase)
            ? CarProductType
            : productType.Equals(PartProductType, StringComparison.OrdinalIgnoreCase)
                ? PartProductType
                : throw new ValidationException("Поддерживаются только типы Car и Part.");

    private static InventoryReservationResponse Map(InventoryReservation reservation) => new()
    {
        OrderId = reservation.OrderId,
        Items = reservation.Items.Select(x => new InventoryReservationItemResponse
        {
            ProductType = x.ProductType,
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            Name = x.Name,
            Article = x.Article,
            BrandName = x.BrandName,
            ModelName = x.ModelName,
            GenerationName = x.GenerationName,
            Year = x.Year
        }).ToList()
    };
}
