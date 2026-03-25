using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Repositories.Implementations;

/// <summary>
/// Реализация репозитория для работы с моделями автомобилей.
/// </summary>
public class CarModelRepository : ICarModelRepository
{
    private readonly CatalogDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория моделей автомобилей.
    /// </summary>
    public CarModelRepository(CatalogDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CarModels
            .AsNoTracking()
            .Include(x => x.Brand)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CarModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CarModels
            .AsNoTracking()
            .Include(x => x.Brand)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CarModel?> GetByNameAsync(string name, Guid brandId, CancellationToken cancellationToken = default)
    {
        return await _context.CarModels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && x.BrandId == brandId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(CarModel model, CancellationToken cancellationToken = default)
    {
        await _context.CarModels.AddAsync(model, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(CarModel model, CancellationToken cancellationToken = default)
    {
        _context.CarModels.Update(model);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await _context.CarModels.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (model is null)
        {
            return;
        }

        _context.CarModels.Remove(model);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarModel>> GetByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        return await _context.CarModels
            .AsNoTracking()
            .Include(x => x.Brand)
            .Where(x => x.BrandId == brandId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}