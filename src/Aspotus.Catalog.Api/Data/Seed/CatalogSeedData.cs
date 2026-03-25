using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Catalog.Api.Data.Seed;

/// <summary>
/// Выполняет начальное заполнение базы данных каталога тестовыми данными.
/// </summary>
public static class CatalogSeedData
{
    /// <summary>
    /// Заполняет базу данных начальными данными, если она ещё пуста.
    /// </summary>
    /// <param name="context">Контекст базы данных каталога.</param>
    public static async Task SeedAsync(CatalogDbContext context)
    {
        if (await context.CarBrands.AnyAsync())
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

        var camryXv70Generation = new CarGeneration
        {
            Id = Guid.NewGuid(),
            Name = "XV70",
            YearFrom = 2017,
            YearTo = null,
            ModelId = camryModel.Id
        };

        var corollaE210Generation = new CarGeneration
        {
            Id = Guid.NewGuid(),
            Name = "E210",
            YearFrom = 2018,
            YearTo = null,
            ModelId = corollaModel.Id
        };

        var x5G05Generation = new CarGeneration
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
            GenerationId = camryXv70Generation.Id,
            Year = 2020,
            Mileage = 118000,
            BodyType = "Sedan",
            TrimLevelName = "Prestige Safety",
            TrimLevelDescription = "Кожаный салон, камера 360, адаптивный круиз-контроль, подогрев сидений.",
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
            GenerationId = corollaE210Generation.Id,
            Year = 2019,
            Mileage = 86000,
            BodyType = "Sedan",
            TrimLevelName = "Comfort",
            TrimLevelDescription = "Кондиционер, мультируль, подогрев передних сидений.",
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
            GenerationId = x5G05Generation.Id,
            Year = 2021,
            Mileage = 64000,
            BodyType = "Suv",
            TrimLevelName = "M Sport",
            TrimLevelDescription = "Спортивный пакет M, панорама, адаптивная подвеска, премиальная акустика.",
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

        var filtersCategory = new PartCategory
        {
            Id = Guid.NewGuid(),
            Name = "Filters",
            ParentCategoryId = engineCategory.Id
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

        var bodyPartsCategory = new PartCategory
        {
            Id = Guid.NewGuid(),
            Name = "Body Parts"
        };

        var toyotaManufacturer = new PartManufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Toyota Genuine Parts"
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

        var usedPartsWarehouseManufacturer = new PartManufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Aspotus Used Parts"
        };

        var oilFilterPart = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Oil Filter",
            Article = "TOY-OF-001",
            Description = "Оригинальный масляный фильтр для бензиновых двигателей Toyota.",
            Price = 18.50m,
            StockQuantity = 45,
            IsOriginal = true,
            ConditionType = PartConditionType.New,
            ConditionPercent = null,
            ConditionDescription = null,
            MileageAtRemoval = null,
            CategoryId = filtersCategory.Id,
            ManufacturerId = toyotaManufacturer.Id,
            ReplacementArticles = new List<PartReplacement>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "90915-YZZE1"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "BOSCH-OF-7788"
                }
            }
        };

        var airFilterPart = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Air Filter",
            Article = "BOS-AF-150",
            Description = "Воздушный фильтр двигателя для Toyota Camry XV70 и Corolla E210.",
            Price = 22.90m,
            StockQuantity = 38,
            IsOriginal = false,
            ConditionType = PartConditionType.New,
            ConditionPercent = null,
            ConditionDescription = null,
            MileageAtRemoval = null,
            CategoryId = filtersCategory.Id,
            ManufacturerId = boschManufacturer.Id,
            ReplacementArticles = new List<PartReplacement>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "17801-25020"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "BOSCH-F026400492"
                }
            }
        };

        var brakePadsPart = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Front Brake Pads",
            Article = "BRE-PAD-330",
            Description = "Передние тормозные колодки для Toyota Camry XV70.",
            Price = 74.99m,
            StockQuantity = 20,
            IsOriginal = false,
            ConditionType = PartConditionType.New,
            ConditionPercent = null,
            ConditionDescription = null,
            MileageAtRemoval = null,
            CategoryId = brakesCategory.Id,
            ManufacturerId = bremboManufacturer.Id,
            ReplacementArticles = new List<PartReplacement>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "04465-33480"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "P83117"
                }
            }
        };

        var shockAbsorberPart = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Rear Shock Absorber",
            Article = "KYB-RS-210",
            Description = "Задний амортизатор для Toyota Corolla E210.",
            Price = 96.00m,
            StockQuantity = 14,
            IsOriginal = false,
            ConditionType = PartConditionType.New,
            ConditionPercent = null,
            ConditionDescription = null,
            MileageAtRemoval = null,
            CategoryId = suspensionCategory.Id,
            ManufacturerId = kybManufacturer.Id,
            ReplacementArticles = new List<PartReplacement>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "48530-02Q90"
                }
            }
        };

        var usedHeadlightPart = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Left Headlight",
            Article = "USED-CAMRY-HL-L-01",
            Description = "Левая передняя фара для Toyota Camry XV70.",
            Price = 210.00m,
            StockQuantity = 1,
            IsOriginal = true,
            ConditionType = PartConditionType.Used,
            ConditionPercent = 78,
            ConditionDescription = "Есть мелкие царапины на стекле, крепления целые, следы эксплуатации.",
            MileageAtRemoval = 112000,
            CategoryId = bodyPartsCategory.Id,
            ManufacturerId = usedPartsWarehouseManufacturer.Id,
            ReplacementArticles = new List<PartReplacement>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "81150-06C40"
                }
            }
        };

        var usedDoorMirrorPart = new Part
        {
            Id = Guid.NewGuid(),
            Name = "Right Door Mirror",
            Article = "USED-BMW-MIR-R-03",
            Description = "Правое зеркало заднего вида для BMW X5 G05.",
            Price = 185.00m,
            StockQuantity = 1,
            IsOriginal = true,
            ConditionType = PartConditionType.Used,
            ConditionPercent = 82,
            ConditionDescription = "Небольшие потёртости корпуса, электропривод исправен, стекло целое.",
            MileageAtRemoval = 64000,
            CategoryId = bodyPartsCategory.Id,
            ManufacturerId = usedPartsWarehouseManufacturer.Id,
            ReplacementArticles = new List<PartReplacement>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReplacementArticle = "51167422716"
                }
            }
        };

        var compatibilities = new List<PartCompatibility>
        {
            new()
            {
                PartId = oilFilterPart.Id,
                CarId = camryCar.Id
            },
            new()
            {
                PartId = airFilterPart.Id,
                CarId = camryCar.Id
            },
            new()
            {
                PartId = brakePadsPart.Id,
                CarId = camryCar.Id
            },
            new()
            {
                PartId = usedHeadlightPart.Id,
                CarId = camryCar.Id
            },
            new()
            {
                PartId = airFilterPart.Id,
                CarId = corollaCar.Id
            },
            new()
            {
                PartId = shockAbsorberPart.Id,
                CarId = corollaCar.Id
            },
            new()
            {
                PartId = usedDoorMirrorPart.Id,
                CarId = x5Car.Id
            }
        };

        await context.CarBrands.AddRangeAsync(toyotaBrand, bmwBrand);
        await context.CarModels.AddRangeAsync(camryModel, corollaModel, x5Model);
        await context.CarGenerations.AddRangeAsync(camryXv70Generation, corollaE210Generation, x5G05Generation);
        await context.Cars.AddRangeAsync(camryCar, corollaCar, x5Car);

        await context.PartCategories.AddRangeAsync(
            engineCategory,
            filtersCategory,
            brakesCategory,
            suspensionCategory,
            bodyPartsCategory);

        await context.PartManufacturers.AddRangeAsync(
            toyotaManufacturer,
            boschManufacturer,
            bremboManufacturer,
            kybManufacturer,
            usedPartsWarehouseManufacturer);

        await context.Parts.AddRangeAsync(
            oilFilterPart,
            airFilterPart,
            brakePadsPart,
            shockAbsorberPart,
            usedHeadlightPart,
            usedDoorMirrorPart);

        await context.PartCompatibilities.AddRangeAsync(compatibilities);

        await context.SaveChangesAsync();
    }
}