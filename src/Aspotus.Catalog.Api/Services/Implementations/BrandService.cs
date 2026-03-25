using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Mappers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;

namespace Aspotus.Catalog.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с марками автомобилей.
/// </summary>
public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса марок автомобилей.
    /// </summary>
    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<BrandResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _brandRepository.GetAllAsync(cancellationToken);
        return entities.Select(BrandMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<BrandResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _brandRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return BrandMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<BrandResponse> CreateAsync(CreateBrandRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedName = request.Name.Trim();

        var existingBrand = await _brandRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (existingBrand is not null)
        {
            throw new AlreadyExistsException($"Бренд с названием '{normalizedName}' уже существует.");
        }

        var entity = new CarBrand
        {
            Id = Guid.NewGuid(),
            Name = normalizedName
        };

        await _brandRepository.AddAsync(entity, cancellationToken);

        return BrandMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<BrandResponse?> UpdateAsync(Guid id, UpdateBrandRequest request, CancellationToken cancellationToken = default)
    {
        var existingBrand = await _brandRepository.GetByIdAsync(id, cancellationToken);

        if (existingBrand is null)
        {
            return null;
        }

        var normalizedName = request.Name.Trim();

        var brandWithSameName = await _brandRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (brandWithSameName is not null && brandWithSameName.Id != id)
        {
            throw new AlreadyExistsException($"Бренд с названием '{normalizedName}' уже существует.");
        }

        var updatedBrand = new CarBrand
        {
            Id = existingBrand.Id,
            Name = normalizedName
        };

        await _brandRepository.UpdateAsync(updatedBrand, cancellationToken);

        return BrandMapper.ToResponse(updatedBrand);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingBrand = await _brandRepository.GetByIdAsync(id, cancellationToken);

        if (existingBrand is null)
        {
            return false;
        }

        await _brandRepository.DeleteAsync(id, cancellationToken);

        return true;
    }
}