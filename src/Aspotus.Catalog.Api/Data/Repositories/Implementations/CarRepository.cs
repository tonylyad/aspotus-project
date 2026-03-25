using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Repositories.Implementations;

/// <summary>
/// Реализация репозитория для работы с автомобилями.
/// </summary>
public class CarRepository : ICarRepository
{
    private readonly CatalogDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория автомобилей.
    /// </summary>
    public CarRepository(CatalogDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Car>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cars
            .AsNoTracking()
            .Include(x => x.Brand)
            .Include(x => x.Model)
            .Include(x => x.Generation)
            .OrderBy(x => x.Year)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Cars
            .AsNoTracking()
            .Include(x => x.Brand)
            .Include(x => x.Model)
            .Include(x => x.Generation)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Car car, CancellationToken cancellationToken = default)
    {
        await _context.Cars.AddAsync(car, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Car car, CancellationToken cancellationToken = default)
    {
        _context.Cars.Update(car);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (car is null)
        {
            return;
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync(cancellationToken);
    }
}