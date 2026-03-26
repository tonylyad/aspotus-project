using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Mappers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;

namespace Aspotus.Catalog.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с моделями автомобилей.
/// </summary>
public class CarModelService : ICarModelService
{
    private readonly ICarModelRepository _carModelRepository;
    private readonly IBrandRepository _brandRepository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса моделей автомобилей.
    /// </summary>
    public CarModelService(
        ICarModelRepository carModelRepository,
        IBrandRepository brandRepository)
    {
        _carModelRepository = carModelRepository;
        _brandRepository = brandRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarModelResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _carModelRepository.GetAllAsync(cancellationToken);
        return entities.Select(CarModelMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<CarModelResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _carModelRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return CarModelMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<CarModelResponse> CreateAsync(CreateCarModelRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedName = request.Name.Trim();

        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

        if (brand is null)
        {
            throw new NotFoundException("Указанная марка автомобиля не существует.");
        }

        var existingModel = await _carModelRepository.GetByNameAsync(normalizedName, request.BrandId, cancellationToken);

        if (existingModel is not null)
        {
            throw new AlreadyExistsException($"Модель '{normalizedName}' уже существует у выбранной марки.");
        }

        var entity = new CarModel
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            BrandId = request.BrandId
        };

        await _carModelRepository.AddAsync(entity, cancellationToken);

        var savedEntity = await _carModelRepository.GetByIdAsync(entity.Id, cancellationToken);

        return CarModelMapper.ToResponse(savedEntity!);
    }

    /// <inheritdoc />
    public async Task<CarModelResponse?> UpdateAsync(Guid id, UpdateCarModelRequest request, CancellationToken cancellationToken = default)
    {
        var existingModel = await _carModelRepository.GetByIdAsync(id, cancellationToken);

        if (existingModel is null)
        {
            return null;
        }

        var normalizedName = request.Name.Trim();

        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

        if (brand is null)
        {
            throw new NotFoundException("Указанная марка автомобиля не существует.");
        }

        var modelWithSameName = await _carModelRepository.GetByNameAsync(normalizedName, request.BrandId, cancellationToken);

        if (modelWithSameName is not null && modelWithSameName.Id != id)
        {
            throw new AlreadyExistsException($"Модель '{normalizedName}' уже существует у выбранной марки.");
        }

        var updatedModel = new CarModel
        {
            Id = existingModel.Id,
            Name = normalizedName,
            BrandId = request.BrandId
        };

        await _carModelRepository.UpdateAsync(updatedModel, cancellationToken);

        var savedEntity = await _carModelRepository.GetByIdAsync(updatedModel.Id, cancellationToken);

        return CarModelMapper.ToResponse(savedEntity!);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingModel = await _carModelRepository.GetByIdAsync(id, cancellationToken);

        if (existingModel is null)
        {
            return false;
        }

        await _carModelRepository.DeleteAsync(id, cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarModelResponse>> GetByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);

        if (brand is null)
        {
            throw new NotFoundException("Указанная марка автомобиля не существует.");
        }

        var entities = await _carModelRepository.GetByBrandIdAsync(brandId, cancellationToken);

        return entities.Select(CarModelMapper.ToResponse).ToList();
    }
}