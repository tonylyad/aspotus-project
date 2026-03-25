using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Repositories.Implementations;

/// <summary>
/// Реализация репозитория для работы с поколениями автомобилей.
/// </summary>
public class CarGenerationRepository : ICarGenerationRepository
{
    private readonly CatalogDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория поколений автомобилей.
    /// </summary>
    public CarGenerationRepository(CatalogDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarGeneration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CarGenerations
            .AsNoTracking()
            .Include(x => x.Model)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CarGeneration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CarGenerations
            .AsNoTracking()
            .Include(x => x.Model)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CarGeneration?> GetByNameAsync(string name, Guid modelId, CancellationToken cancellationToken = default)
    {
        return await _context.CarGenerations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && x.ModelId == modelId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(CarGeneration generation, CancellationToken cancellationToken = default)
    {
        await _context.CarGenerations.AddAsync(generation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(CarGeneration generation, CancellationToken cancellationToken = default)
    {
        _context.CarGenerations.Update(generation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var generation = await _context.CarGenerations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (generation is null)
        {
            return;
        }

        _context.CarGenerations.Remove(generation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarGeneration>> GetByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        return await _context.CarGenerations
            .AsNoTracking()
            .Include(x => x.Model)
            .Where(x => x.ModelId == modelId)
            .OrderBy(x => x.YearFrom)
            .ToListAsync(cancellationToken);
    }
}