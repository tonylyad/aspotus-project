using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Mappers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;

namespace Aspotus.Catalog.Api.Services.Implementations;

/// <summary>
/// Реализация бизнес-логики для работы с производителями запчастей.
/// </summary>
public class PartManufacturerService : IPartManufacturerService
{
    private readonly IPartManufacturerRepository _partManufacturerRepository;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса производителей запчастей.
    /// </summary>
    public PartManufacturerService(IPartManufacturerRepository partManufacturerRepository)
    {
        _partManufacturerRepository = partManufacturerRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PartManufacturerResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _partManufacturerRepository.GetAllAsync(cancellationToken);
        return entities.Select(PartManufacturerMapper.ToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<PartManufacturerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _partManufacturerRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return PartManufacturerMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<PartManufacturerResponse> CreateAsync(CreatePartManufacturerRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedName = request.Name.Trim();

        var existingManufacturer = await _partManufacturerRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (existingManufacturer is not null)
        {
            throw new AlreadyExistsException($"Производитель с названием '{normalizedName}' уже существует.");
        }

        var entity = new PartManufacturer
        {
            Id = Guid.NewGuid(),
            Name = normalizedName
        };

        await _partManufacturerRepository.AddAsync(entity, cancellationToken);

        return PartManufacturerMapper.ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<PartManufacturerResponse?> UpdateAsync(Guid id, UpdatePartManufacturerRequest request, CancellationToken cancellationToken = default)
    {
        var existingManufacturer = await _partManufacturerRepository.GetByIdAsync(id, cancellationToken);

        if (existingManufacturer is null)
        {
            return null;
        }

        var normalizedName = request.Name.Trim();

        var manufacturerWithSameName = await _partManufacturerRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (manufacturerWithSameName is not null && manufacturerWithSameName.Id != id)
        {
            throw new AlreadyExistsException($"Производитель с названием '{normalizedName}' уже существует.");
        }

        var updatedManufacturer = new PartManufacturer
        {
            Id = existingManufacturer.Id,
            Name = normalizedName
        };

        await _partManufacturerRepository.UpdateAsync(updatedManufacturer, cancellationToken);

        return PartManufacturerMapper.ToResponse(updatedManufacturer);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingManufacturer = await _partManufacturerRepository.GetByIdAsync(id, cancellationToken);

        if (existingManufacturer is null)
        {
            return false;
        }

        await _partManufacturerRepository.DeleteAsync(id, cancellationToken);

        return true;
    }
}