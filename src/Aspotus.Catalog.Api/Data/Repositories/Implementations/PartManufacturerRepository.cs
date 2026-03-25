using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Repositories.Implementations;

/// <summary>
/// Реализация репозитория для работы с производителями запчастей.
/// </summary>
public class PartManufacturerRepository : IPartManufacturerRepository
{
    private readonly CatalogDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория производителей запчастей.
    /// </summary>
    public PartManufacturerRepository(CatalogDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PartManufacturer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PartManufacturers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PartManufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PartManufacturers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PartManufacturer?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.PartManufacturers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(PartManufacturer manufacturer, CancellationToken cancellationToken = default)
    {
        await _context.PartManufacturers.AddAsync(manufacturer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(PartManufacturer manufacturer, CancellationToken cancellationToken = default)
    {
        _context.PartManufacturers.Update(manufacturer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var manufacturer = await _context.PartManufacturers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (manufacturer is null)
        {
            return;
        }

        _context.PartManufacturers.Remove(manufacturer);
        await _context.SaveChangesAsync(cancellationToken);
    }
}