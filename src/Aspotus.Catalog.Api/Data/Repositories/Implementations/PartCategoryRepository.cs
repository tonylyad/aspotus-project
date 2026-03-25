using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Repositories.Implementations;

/// <summary>
/// Реализация репозитория для работы с категориями запчастей.
/// </summary>
public class PartCategoryRepository : IPartCategoryRepository
{
    private readonly CatalogDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория категорий запчастей.
    /// </summary>
    public PartCategoryRepository(CatalogDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PartCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PartCategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PartCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PartCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PartCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.PartCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(PartCategory category, CancellationToken cancellationToken = default)
    {
        await _context.PartCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(PartCategory category, CancellationToken cancellationToken = default)
    {
        _context.PartCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.PartCategories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (category is null)
        {
            return;
        }

        _context.PartCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}