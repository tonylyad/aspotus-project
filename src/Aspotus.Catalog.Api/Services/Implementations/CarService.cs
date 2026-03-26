using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Mappers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;

namespace Aspotus.Catalog.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с автомобилями.
/// </summary>
public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICarModelRepository _carModelRepository;
    private readonly ICarGenerationRepository _carGenerationRepository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса автомобилей.
    /// </summary>
    public CarService(
        ICarRepository carRepository,
        IBrandRepository brandRepository,
        ICarModelRepository carModelRepository,
        ICarGenerationRepository carGenerationRepository)
    {
        _carRepository = carRepository;
        _brandRepository = brandRepository;
        _carModelRepository = carModelRepository;
        _carGenerationRepository = carGenerationRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CarResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _carRepository.GetAllAsync(cancellationToken);
        return entities.Select(CarMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<CarResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _carRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return CarMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<CarResponse> CreateAsync(CreateCarRequest request, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

        if (brand is null)
        {
            throw new NotFoundException("Указанная марка автомобиля не существует.");
        }

        var model = await _carModelRepository.GetByIdAsync(request.ModelId, cancellationToken);

        if (model is null)
        {
            throw new NotFoundException("Указанная модель автомобиля не существует.");
        }

        var generation = await _carGenerationRepository.GetByIdAsync(request.GenerationId, cancellationToken);

        if (generation is null)
        {
            throw new NotFoundException("Указанное поколение автомобиля не существует.");
        }

        if (model.BrandId != request.BrandId)
        {
            throw new ValidationException("Указанная модель не принадлежит выбранной марке автомобиля.");
        }

        if (generation.ModelId != request.ModelId)
        {
            throw new ValidationException("Указанное поколение не принадлежит выбранной модели автомобиля.");
        }

        var entity = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = request.BrandId,
            ModelId = request.ModelId,
            GenerationId = request.GenerationId,
            Year = request.Year,
            Mileage = request.Mileage,
            BodyType = request.BodyType.Trim(),
            TrimLevelName = string.IsNullOrWhiteSpace(request.TrimLevelName) ? null : request.TrimLevelName.Trim(),
            TrimLevelDescription = string.IsNullOrWhiteSpace(request.TrimLevelDescription) ? null : request.TrimLevelDescription.Trim(),
            EngineVolume = request.EngineVolume,
            FuelType = request.FuelType.Trim(),
            TransmissionType = request.TransmissionType.Trim(),
            DriveType = request.DriveType.Trim()
        };

        await _carRepository.AddAsync(entity, cancellationToken);

        var savedEntity = await _carRepository.GetByIdAsync(entity.Id, cancellationToken);

        return CarMapper.ToResponse(savedEntity!);
    }

    /// <inheritdoc />
    public async Task<CarResponse?> UpdateAsync(Guid id, UpdateCarRequest request, CancellationToken cancellationToken = default)
    {
        var existingCar = await _carRepository.GetByIdAsync(id, cancellationToken);

        if (existingCar is null)
        {
            return null;
        }

        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

        if (brand is null)
        {
            throw new NotFoundException("Указанная марка автомобиля не существует.");
        }

        var model = await _carModelRepository.GetByIdAsync(request.ModelId, cancellationToken);

        if (model is null)
        {
            throw new NotFoundException("Указанная модель автомобиля не существует.");
        }

        var generation = await _carGenerationRepository.GetByIdAsync(request.GenerationId, cancellationToken);

        if (generation is null)
        {
            throw new NotFoundException("Указанное поколение автомобиля не существует.");
        }

        if (model.BrandId != request.BrandId)
        {
            throw new ValidationException("Указанная модель не принадлежит выбранной марке автомобиля.");
        }

        if (generation.ModelId != request.ModelId)
        {
            throw new ValidationException("Указанное поколение не принадлежит выбранной модели автомобиля.");
        }

        var updatedCar = new Car
        {
            Id = existingCar.Id,
            BrandId = request.BrandId,
            ModelId = request.ModelId,
            GenerationId = request.GenerationId,
            Year = request.Year,
            Mileage = request.Mileage,
            BodyType = request.BodyType.Trim(),
            TrimLevelName = string.IsNullOrWhiteSpace(request.TrimLevelName) ? null : request.TrimLevelName.Trim(),
            TrimLevelDescription = string.IsNullOrWhiteSpace(request.TrimLevelDescription) ? null : request.TrimLevelDescription.Trim(),
            EngineVolume = request.EngineVolume,
            FuelType = request.FuelType.Trim(),
            TransmissionType = request.TransmissionType.Trim(),
            DriveType = request.DriveType.Trim()
        };

        await _carRepository.UpdateAsync(updatedCar, cancellationToken);

        var savedEntity = await _carRepository.GetByIdAsync(updatedCar.Id, cancellationToken);

        return CarMapper.ToResponse(savedEntity!);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingCar = await _carRepository.GetByIdAsync(id, cancellationToken);

        if (existingCar is null)
        {
            return false;
        }

        await _carRepository.DeleteAsync(id, cancellationToken);

        return true;
    }
}