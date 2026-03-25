using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Mappers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;

namespace Aspotus.Catalog.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с поколениями автомобилей.
/// </summary>
public class CarGenerationService : ICarGenerationService
{
    private readonly ICarGenerationRepository _carGenerationRepository;
    private readonly ICarModelRepository _carModelRepository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса поколений автомобилей.
    /// </summary>
    public CarGenerationService(
        ICarGenerationRepository carGenerationRepository,
        ICarModelRepository carModelRepository)
    {
        _carGenerationRepository = carGenerationRepository;
        _carModelRepository = carModelRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarGenerationResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _carGenerationRepository.GetAllAsync(cancellationToken);
        return entities.Select(CarGenerationMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<CarGenerationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _carGenerationRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return CarGenerationMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<CarGenerationResponse> CreateAsync(CreateCarGenerationRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedName = request.Name.Trim();

        var model = await _carModelRepository.GetByIdAsync(request.ModelId, cancellationToken);

        if (model is null)
        {
            throw new NotFoundException("Указанная модель автомобиля не существует.");
        }

        var existingGeneration = await _carGenerationRepository.GetByNameAsync(normalizedName, request.ModelId, cancellationToken);

        if (existingGeneration is not null)
        {
            throw new AlreadyExistsException($"Поколение '{normalizedName}' уже существует у выбранной модели.");
        }

        var entity = new CarGeneration
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            YearFrom = request.YearFrom,
            YearTo = request.YearTo,
            ModelId = request.ModelId,
            Model = new CarModel
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId,
                Brand = model.Brand
            }
        };

        await _carGenerationRepository.AddAsync(entity, cancellationToken);

        return CarGenerationMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<CarGenerationResponse?> UpdateAsync(Guid id, UpdateCarGenerationRequest request, CancellationToken cancellationToken = default)
    {
        var existingGeneration = await _carGenerationRepository.GetByIdAsync(id, cancellationToken);

        if (existingGeneration is null)
        {
            return null;
        }

        var normalizedName = request.Name.Trim();

        var model = await _carModelRepository.GetByIdAsync(request.ModelId, cancellationToken);

        if (model is null)
        {
            throw new NotFoundException("Указанная модель автомобиля не существует.");
        }

        var generationWithSameName = await _carGenerationRepository.GetByNameAsync(normalizedName, request.ModelId, cancellationToken);

        if (generationWithSameName is not null && generationWithSameName.Id != id)
        {
            throw new AlreadyExistsException($"Поколение '{normalizedName}' уже существует у выбранной модели.");
        }

        var updatedGeneration = new CarGeneration
        {
            Id = existingGeneration.Id,
            Name = normalizedName,
            YearFrom = request.YearFrom,
            YearTo = request.YearTo,
            ModelId = request.ModelId,
            Model = new CarModel
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId,
                Brand = model.Brand
            }
        };

        await _carGenerationRepository.UpdateAsync(updatedGeneration, cancellationToken);

        return CarGenerationMapper.ToResponse(updatedGeneration);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingGeneration = await _carGenerationRepository.GetByIdAsync(id, cancellationToken);

        if (existingGeneration is null)
        {
            return false;
        }

        await _carGenerationRepository.DeleteAsync(id, cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarGenerationResponse>> GetByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        var model = await _carModelRepository.GetByIdAsync(modelId, cancellationToken);

        if (model is null)
        {
            throw new NotFoundException("Указанная модель автомобиля не существует.");
        }

        var entities = await _carGenerationRepository.GetByModelIdAsync(modelId, cancellationToken);

        return entities.Select(CarGenerationMapper.ToResponse).ToList();
    }
}