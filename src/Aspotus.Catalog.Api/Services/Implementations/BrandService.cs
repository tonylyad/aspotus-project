using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Mappers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Options;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Aspotus.Catalog.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с марками автомобилей.
/// </summary>
public class BrandService : IBrandService
{
    private const string AllBrandsCacheKey = "catalog:brands:all";

    private readonly IBrandRepository _brandRepository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<BrandService> _logger;
    private readonly DistributedCacheEntryOptions _cacheEntryOptions;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса марок автомобилей.
    /// </summary>
    public BrandService(
        IBrandRepository brandRepository,
        IDistributedCache cache,
        IOptions<BrandCacheOptions> cacheOptions,
        ILogger<BrandService> logger)
    {
        _brandRepository = brandRepository;
        _cache = cache;
        _logger = logger;
        _cacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                Math.Max(1, cacheOptions.Value.BrandsExpirationMinutes))
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<BrandResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cachedBrands = await GetFromCacheAsync<List<BrandResponse>>(
            AllBrandsCacheKey,
            cancellationToken);

        if (cachedBrands is not null)
        {
            return cachedBrands;
        }

        var entities = await _brandRepository.GetAllAsync(cancellationToken);
        var brands = entities.Select(BrandMapper.ToResponse).ToList();

        await SetCacheAsync(AllBrandsCacheKey, brands, cancellationToken);

        return brands;
    }

    /// <inheritdoc />
    public async Task<BrandResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetBrandCacheKey(id);
        var cachedBrand = await GetFromCacheAsync<BrandResponse>(cacheKey, cancellationToken);

        if (cachedBrand is not null)
        {
            return cachedBrand;
        }

        var entity = await _brandRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var brand = BrandMapper.ToResponse(entity);
        await SetCacheAsync(cacheKey, brand, cancellationToken);

        return brand;
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
        await RemoveCacheAsync(AllBrandsCacheKey, cancellationToken);

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
        await RemoveCacheAsync(AllBrandsCacheKey, cancellationToken);
        await RemoveCacheAsync(GetBrandCacheKey(id), cancellationToken);

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
        await RemoveCacheAsync(AllBrandsCacheKey, cancellationToken);
        await RemoveCacheAsync(GetBrandCacheKey(id), cancellationToken);

        return true;
    }

    private static string GetBrandCacheKey(Guid id) => $"catalog:brands:{id}";

    private async Task<T?> GetFromCacheAsync<T>(
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            var value = await _cache.GetAsync(key, cancellationToken);
            return value is null ? default : JsonSerializer.Deserialize<T>(value);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Не удалось прочитать ключ {CacheKey} из Redis.", key);
            return default;
        }
    }

    private async Task SetCacheAsync<T>(
        string key,
        T value,
        CancellationToken cancellationToken)
    {
        try
        {
            var serializedValue = JsonSerializer.SerializeToUtf8Bytes(value);
            await _cache.SetAsync(key, serializedValue, _cacheEntryOptions, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Не удалось записать ключ {CacheKey} в Redis.", key);
        }
    }

    private async Task RemoveCacheAsync(
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Не удалось удалить ключ {CacheKey} из Redis.", key);
        }
    }
}
