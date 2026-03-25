using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Enums;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Mappers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;

namespace Aspotus.Catalog.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с запчастями.
/// </summary>
public class PartService : IPartService
{
    private readonly IPartRepository _partRepository;
    private readonly IPartCategoryRepository _partCategoryRepository;
    private readonly IPartManufacturerRepository _partManufacturerRepository;
    private readonly ICarRepository _carRepository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса запчастей.
    /// </summary>
    public PartService(
        IPartRepository partRepository,
        IPartCategoryRepository partCategoryRepository,
        IPartManufacturerRepository partManufacturerRepository,
        ICarRepository carRepository)
    {
        _partRepository = partRepository;
        _partCategoryRepository = partCategoryRepository;
        _partManufacturerRepository = partManufacturerRepository;
        _carRepository = carRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PartResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _partRepository.GetAllAsync(cancellationToken);
        return entities.Select(PartMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<PartResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _partRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return PartMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PartResponse>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _partCategoryRepository.GetByIdAsync(categoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Указанная категория запчастей не существует.");
        }

        var entities = await _partRepository.GetByCategoryIdAsync(categoryId, cancellationToken);

        return entities.Select(PartMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PartResponse>> GetByCarIdAsync(Guid carId, CancellationToken cancellationToken = default)
    {
        var car = await _carRepository.GetByIdAsync(carId, cancellationToken);

        if (car is null)
        {
            throw new NotFoundException("Указанный автомобиль не существует.");
        }

        var entities = await _partRepository.GetByCarIdAsync(carId, cancellationToken);

        return entities.Select(PartMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<PartResponse> CreateAsync(CreatePartRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedName = request.Name.Trim();
        var normalizedArticle = request.Article.Trim();

        var existingPart = await _partRepository.GetByArticleAsync(normalizedArticle, cancellationToken);

        if (existingPart is not null)
        {
            throw new AlreadyExistsException($"Запчасть с артикулом '{normalizedArticle}' уже существует.");
        }

        var category = await _partCategoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Указанная категория запчасти не существует.");
        }

        var manufacturer = await _partManufacturerRepository.GetByIdAsync(request.ManufacturerId, cancellationToken);

        if (manufacturer is null)
        {
            throw new NotFoundException("Указанный производитель запчасти не существует.");
        }

        ValidatePartState(request.ConditionType, request.ConditionPercent, request.ConditionDescription, request.MileageAtRemoval);

        var entity = new Part
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Article = normalizedArticle,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsOriginal = request.IsOriginal,
            ConditionType = request.ConditionType,
            ConditionPercent = request.ConditionPercent,
            ConditionDescription = string.IsNullOrWhiteSpace(request.ConditionDescription) ? null : request.ConditionDescription.Trim(),
            MileageAtRemoval = request.MileageAtRemoval,
            CategoryId = request.CategoryId,
            ManufacturerId = request.ManufacturerId,
            ReplacementArticles = request.ReplacementArticles
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(x => new PartReplacement
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = x
                })
                .ToList()
        };

        await _partRepository.AddAsync(entity, cancellationToken);

        var savedEntity = await _partRepository.GetByIdAsync(entity.Id, cancellationToken);

        return PartMapper.ToResponse(savedEntity!);
    }

    /// <inheritdoc />
    public async Task<PartResponse?> UpdateAsync(Guid id, UpdatePartRequest request, CancellationToken cancellationToken = default)
    {
        var existingPart = await _partRepository.GetByIdAsync(id, cancellationToken);

        if (existingPart is null)
        {
            return null;
        }

        var normalizedName = request.Name.Trim();
        var normalizedArticle = request.Article.Trim();

        var partWithSameArticle = await _partRepository.GetByArticleAsync(normalizedArticle, cancellationToken);

        if (partWithSameArticle is not null && partWithSameArticle.Id != id)
        {
            throw new AlreadyExistsException($"Запчасть с артикулом '{normalizedArticle}' уже существует.");
        }

        var category = await _partCategoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Указанная категория запчасти не существует.");
        }

        var manufacturer = await _partManufacturerRepository.GetByIdAsync(request.ManufacturerId, cancellationToken);

        if (manufacturer is null)
        {
            throw new NotFoundException("Указанный производитель запчасти не существует.");
        }

        ValidatePartState(request.ConditionType, request.ConditionPercent, request.ConditionDescription, request.MileageAtRemoval);

        var updatedPart = new Part
        {
            Id = existingPart.Id,
            Name = normalizedName,
            Article = normalizedArticle,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsOriginal = request.IsOriginal,
            ConditionType = request.ConditionType,
            ConditionPercent = request.ConditionPercent,
            ConditionDescription = string.IsNullOrWhiteSpace(request.ConditionDescription) ? null : request.ConditionDescription.Trim(),
            MileageAtRemoval = request.MileageAtRemoval,
            CategoryId = request.CategoryId,
            ManufacturerId = request.ManufacturerId,
            ReplacementArticles = request.ReplacementArticles
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(x => new PartReplacement
                {
                    Id = Guid.NewGuid(),
                    PartId = existingPart.Id,
                    ReplacementArticle = x
                })
                .ToList()
        };

        await _partRepository.UpdateAsync(updatedPart, cancellationToken);

        var savedEntity = await _partRepository.GetByIdAsync(updatedPart.Id, cancellationToken);

        return PartMapper.ToResponse(savedEntity!);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingPart = await _partRepository.GetByIdAsync(id, cancellationToken);

        if (existingPart is null)
        {
            return false;
        }

        await _partRepository.DeleteAsync(id, cancellationToken);

        return true;
    }

    private static void ValidatePartState(
        PartConditionType conditionType,
        int? conditionPercent,
        string? conditionDescription,
        int? mileageAtRemoval)
    {
        if (conditionType == PartConditionType.New)
        {
            if (conditionPercent.HasValue || !string.IsNullOrWhiteSpace(conditionDescription) || mileageAtRemoval.HasValue)
            {
                throw new ValidationException("Для новой запчасти нельзя указывать состояние, описание состояния и пробег снятия.");
            }
        }

        if (conditionType == PartConditionType.Used)
        {
            if (!conditionPercent.HasValue)
            {
                throw new ValidationException("Для БУ-запчасти необходимо указать процент состояния.");
            }
        }
    }
}