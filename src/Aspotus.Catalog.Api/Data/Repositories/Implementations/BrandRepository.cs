using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Repositories.Implementations;

/// <summary>
/// Реализация репозитория для работы с марками автомобилей.
/// </summary>
public class BrandRepository : IBrandRepository
{
    private readonly CatalogDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория марок автомобилей.
    /// </summary>
    public BrandRepository(CatalogDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarBrand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CarBrands
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CarBrand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CarBrands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CarBrand?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.CarBrands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(CarBrand brand, CancellationToken cancellationToken = default)
    {
        await _context.CarBrands.AddAsync(brand, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(CarBrand brand, CancellationToken cancellationToken = default)
    {
        _context.CarBrands.Update(brand);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var brand = await _context.CarBrands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (brand is null)
        {
            return;
        }

        _context.CarBrands.Remove(brand);
        await _context.SaveChangesAsync(cancellationToken);
    }
}