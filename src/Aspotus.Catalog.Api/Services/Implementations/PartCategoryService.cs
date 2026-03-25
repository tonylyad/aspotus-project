using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Mappers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;

namespace Aspotus.Catalog.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с категориями запчастей.
/// </summary>
public class PartCategoryService : IPartCategoryService
{
    private readonly IPartCategoryRepository _partCategoryRepository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса категорий запчастей.
    /// </summary>
    public PartCategoryService(IPartCategoryRepository partCategoryRepository)
    {
        _partCategoryRepository = partCategoryRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PartCategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _partCategoryRepository.GetAllAsync(cancellationToken);
        return entities.Select(PartCategoryMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<PartCategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _partCategoryRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return PartCategoryMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<PartCategoryResponse> CreateAsync(CreatePartCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedName = request.Name.Trim();

        var existingCategory = await _partCategoryRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (existingCategory is not null)
        {
            throw new AlreadyExistsException($"Категория с названием '{normalizedName}' уже существует.");
        }

        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _partCategoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);

            if (parentCategory is null)
            {
                throw new NotFoundException("Указанная родительская категория не существует.");
            }
        }

        var entity = new PartCategory
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            ParentCategoryId = request.ParentCategoryId
        };

        await _partCategoryRepository.AddAsync(entity, cancellationToken);

        return PartCategoryMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<PartCategoryResponse?> UpdateAsync(Guid id, UpdatePartCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var existingCategory = await _partCategoryRepository.GetByIdAsync(id, cancellationToken);

        if (existingCategory is null)
        {
            return null;
        }

        var normalizedName = request.Name.Trim();

        var categoryWithSameName = await _partCategoryRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (categoryWithSameName is not null && categoryWithSameName.Id != id)
        {
            throw new AlreadyExistsException($"Категория с названием '{normalizedName}' уже существует.");
        }

        if (request.ParentCategoryId == id)
        {
            throw new ValidationException("Категория не может быть родительской сама для себя.");
        }

        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _partCategoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);

            if (parentCategory is null)
            {
                throw new NotFoundException("Указанная родительская категория не существует.");
            }
        }

        var updatedCategory = new PartCategory
        {
            Id = existingCategory.Id,
            Name = normalizedName,
            ParentCategoryId = request.ParentCategoryId
        };

        await _partCategoryRepository.UpdateAsync(updatedCategory, cancellationToken);

        return PartCategoryMapper.ToResponse(updatedCategory);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingCategory = await _partCategoryRepository.GetByIdAsync(id, cancellationToken);

        if (existingCategory is null)
        {
            return false;
        }

        await _partCategoryRepository.DeleteAsync(id, cancellationToken);

        return true;
    }
}