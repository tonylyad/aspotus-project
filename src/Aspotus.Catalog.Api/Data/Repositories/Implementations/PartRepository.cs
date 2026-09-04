using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Repositories.Implementations;

/// <summary>
/// Реализация репозитория для работы с запчастями.
/// </summary>
public class PartRepository : IPartRepository
{
    private readonly CatalogDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория запчастей.
    /// </summary>
    public PartRepository(CatalogDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Part>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Manufacturer)
            .Include(x => x.ReplacementArticles)
            .Include(x => x.Images)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Part?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Manufacturer)
            .Include(x => x.ReplacementArticles)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Part?> GetByArticleAsync(string article, CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Article == article, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Part>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Manufacturer)
            .Include(x => x.ReplacementArticles)
            .Include(x => x.Images)
            .Where(x => x.CategoryId == categoryId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Part>> GetByCarIdAsync(Guid carId, CancellationToken cancellationToken = default)
    {
        return await _context.Parts
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Manufacturer)
            .Include(x => x.ReplacementArticles)
            .Include(x => x.Images)
            .Include(x => x.PartCompatibilities)
            .Where(x => x.PartCompatibilities.Any(pc => pc.CarId == carId))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Part part, CancellationToken cancellationToken = default)
    {
        await _context.Parts.AddAsync(part, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Part part, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Parts
            .Include(x => x.ReplacementArticles)
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == part.Id, cancellationToken);

        _context.Entry(existing).CurrentValues.SetValues(part);
        _context.PartReplacements.RemoveRange(existing.ReplacementArticles);
        _context.PartImages.RemoveRange(existing.Images);
        await _context.PartReplacements.AddRangeAsync(part.ReplacementArticles, cancellationToken);
        await _context.PartImages.AddRangeAsync(part.Images, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var part = await _context.Parts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (part is null)
        {
            return;
        }

        _context.Parts.Remove(part);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
