using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;

namespace Aspotus.Catalog.Api.Data.Seed;

public static class CatalogSeedData
{
    public static async Task SeedAsync(CatalogDbContext context)
    {
        if (context.CarBrands.Any() || context.Parts.Any())
        {
            return;
        }

        var toyotaBrand = new CarBrand
        {
            Id = Guid.NewGuid(),
            Name = "Toyota"
        };

        var bmwBrand = new CarBrand
        {
            Id = Guid.NewGuid(),
            Name = "BMW"
        };

        var camryModel = new CarModel
        {
            Id = Guid.NewGuid(),
            Name = "Camry",
            BrandId = toyotaBrand.Id
        };

        var corollaModel = new CarModel
        {
            Id = Guid.NewGuid(),
            Name = "Corolla",
            BrandId = toyotaBrand.Id
        };

        var x5Model = new CarModel
        {
            Id = Guid.NewGuid(),
            Name = "X5",
            BrandId = bmwBrand.Id
        };

        var camryGeneration = new CarGeneration
        {
            Id = Guid.NewGuid(),
            Name = "XV70",
            YearFrom = 2017,
            YearTo = null,
            ModelId = camryModel.Id
        };

        var corollaGeneration = new CarGeneration
        {
            Id = Guid.NewGuid(),
            Name = "E210",
            YearFrom = 2018,
            YearTo = null,
            ModelId = corollaModel.Id
        };

        var x5Generation = new CarGeneration
        {
            Id = Guid.NewGuid(),
            Name = "G05",
            YearFrom = 2018,
            YearTo = null,
            ModelId = x5Model.Id
        };

        var camryCar = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = toyotaBrand.Id,
            ModelId = camryModel.Id,
            GenerationId = camryGeneration.Id,
            Year = 2020,
            BodyType = "Sedan",
            EngineVolume = 2.5m,
            FuelType = "Petrol",
            TransmissionType = "Automatic",
            DriveType = "Fwd"
        };

        var corollaCar = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = toyotaBrand.Id,
            ModelId = corollaModel.Id,
            GenerationId = corollaGeneration.Id,
            Year = 2021,
            BodyType = "Sedan",
            EngineVolume = 1.6m,
            FuelType = "Petrol",
            TransmissionType = "Automatic",
            DriveType = "Fwd"
        };

        var x5Car = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = bmwBrand.Id,
            ModelId = x5Model.Id,
            GenerationId = x5Generation.Id,
            Year = 2022,
            BodyType = "Suv",
            EngineVolume = 3.0m,
            FuelType = "Diesel",
            TransmissionType = "Automatic",
            DriveType = "Awd"
        };

        var engineCategory = new PartCategory
        {
            Id = Guid.NewGuid(),
            Name = "Engine"
        };

        var brakesCategory = new PartCategory
        {
            Id = Guid.NewGuid(),
            Name = "Brakes"
        };

        var suspensionCategory = new PartCategory
        {
            Id = Guid.NewGuid(),
            Name = "Suspension"
        };

        var boschManufacturer = new PartManufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Bosch"
        };

        var bremboManufacturer = new PartManufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Brembo"
        };

        var kybManufacturer = new PartManufacturer
        {
            Id = Guid.NewGuid(),
            Name = "KYB"
        };

        var oilFilter = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Oil Filter",
            Article = "OF-001",
            Description = "Standard oil filter",
            Price = 15.99m,
            StockQuantity = 50,
            IsOriginal = false,
            CategoryId = engineCategory.Id,
            ManufacturerId = boschManufacturer.Id
        };

        var brakePads = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Brake Pads",
            Article = "BP-101",
            Description = "Front brake pads",
            Price = 79.99m,
            StockQuantity = 30,
            IsOriginal = false,
            CategoryId = brakesCategory.Id,
            ManufacturerId = bremboManufacturer.Id
        };

        var shockAbsorber = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Shock Absorber",
            Article = "SA-777",
            Description = "Rear shock absorber",
            Price = 120.50m,
            StockQuantity = 20,
            IsOriginal = false,
            CategoryId = suspensionCategory.Id,
            ManufacturerId = kybManufacturer.Id
        };

        var compatibilities = new List<PartCompatibility>
        {
            new()
            {
                PartId = oilFilter.Id,
                CarId = camryCar.Id
            },
            new()
            {
                PartId = oilFilter.Id,
                CarId = corollaCar.Id
            },
            new()
            {
                PartId = brakePads.Id,
                CarId = camryCar.Id
            },
            new()
            {
                PartId = brakePads.Id,
                CarId = x5Car.Id
            },
            new()
            {
                PartId = shockAbsorber.Id,
                CarId = x5Car.Id
            }
        };

        await context.CarBrands.AddRangeAsync(toyotaBrand, bmwBrand);
        await context.CarModels.AddRangeAsync(camryModel, corollaModel, x5Model);
        await context.CarGenerations.AddRangeAsync(camryGeneration, corollaGeneration, x5Generation);
        await context.Cars.AddRangeAsync(camryCar, corollaCar, x5Car);

        await context.PartCategories.AddRangeAsync(engineCategory, brakesCategory, suspensionCategory);
        await context.PartManufacturers.AddRangeAsync(boschManufacturer, bremboManufacturer, kybManufacturer);
        await context.Parts.AddRangeAsync(oilFilter, brakePads, shockAbsorber);

        await context.PartCompatibilities.AddRangeAsync(compatibilities);

        await context.SaveChangesAsync();
    }
}