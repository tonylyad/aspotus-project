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
            await AttachImagesToExistingSeedDataAsync(context);
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
            Id = Guid.Parse("4f41e6ed-023e-47ce-953b-0bbf1c6faf84"),
            BrandId = toyotaBrand.Id,
            ModelId = camryModel.Id,
            GenerationId = camryXv70Generation.Id,
            Year = 2020,
            Mileage = 118000,
            Price = 14_900_000m,
            BodyType = "Sedan",
            TrimLevelName = "Prestige Safety",
            TrimLevelDescription = "Кожаный салон, камера 360, адаптивный круиз-контроль, подогрев сидений.",
            EngineVolume = 2.5m,
            FuelType = "Petrol",
            TransmissionType = "Automatic",
            DriveType = "Fwd",
            Images = CreateCarImages(
                Guid.Parse("4f41e6ed-023e-47ce-953b-0bbf1c6faf84"),
                "4f41e6ed-023e-47ce-953b-0bbf1c6faf84",
                5)
        };

        var corollaCar = new Car
        {
            Id = Guid.Parse("9815ff46-c614-4a98-b1e0-7ad7bf5453a2"),
            BrandId = toyotaBrand.Id,
            ModelId = corollaModel.Id,
            GenerationId = corollaE210Generation.Id,
            Year = 2019,
            Mileage = 86000,
            Price = 10_500_000m,
            BodyType = "Sedan",
            TrimLevelName = "Comfort",
            TrimLevelDescription = "Кондиционер, мультируль, подогрев передних сидений.",
            EngineVolume = 1.6m,
            FuelType = "Petrol",
            TransmissionType = "Automatic",
            DriveType = "Fwd",
            Images = CreateCarImages(
                Guid.Parse("9815ff46-c614-4a98-b1e0-7ad7bf5453a2"),
                "9815ff46-c614-4a98-b1e0-7ad7bf5453a2",
                5)
        };

        var x5Car = new Car
        {
            Id = Guid.Parse("dae455c6-22a7-4e90-822d-3796a557bf9a"),
            BrandId = bmwBrand.Id,
            ModelId = x5Model.Id,
            GenerationId = x5G05Generation.Id,
            Year = 2021,
            Mileage = 64000,
            Price = 31_900_000m,
            BodyType = "Suv",
            TrimLevelName = "M Sport",
            TrimLevelDescription = "Спортивный пакет M, панорама, адаптивная подвеска, премиальная акустика.",
            EngineVolume = 3.0m,
            FuelType = "Diesel",
            TransmissionType = "Automatic",
            DriveType = "Awd",
            Images = CreateCarImages(
                Guid.Parse("dae455c6-22a7-4e90-822d-3796a557bf9a"),
                "dae455c6-22a7-4e90-822d-3796a557bf9a",
                5)
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
            Id = Guid.Parse("c585f6bf-8521-4f7b-904b-8dc91b8721d9"),
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
            Images = CreatePartImages(
                Guid.Parse("c585f6bf-8521-4f7b-904b-8dc91b8721d9"),
                "c585f6bf-8521-4f7b-904b-8dc91b8721d9",
                4),
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
            Id = Guid.Parse("f6df2972-c094-4cab-bfeb-0becdf8ac876"),
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
            Images = CreatePartImages(
                Guid.Parse("f6df2972-c094-4cab-bfeb-0becdf8ac876"),
                "f6df2972-c094-4cab-bfeb-0becdf8ac876",
                4),
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
            Id = Guid.Parse("44c41429-a0c8-4d9d-8dca-b30b8fcb8556"),
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
            Images = CreatePartImages(
                Guid.Parse("44c41429-a0c8-4d9d-8dca-b30b8fcb8556"),
                "44c41429-a0c8-4d9d-8dca-b30b8fcb8556",
                4),
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
            Id = Guid.Parse("59d52775-db46-483b-8f81-619b5e7b80df"),
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
            Images = CreatePartImages(
                Guid.Parse("59d52775-db46-483b-8f81-619b5e7b80df"),
                "59d52775-db46-483b-8f81-619b5e7b80df",
                4),
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
            Id = Guid.Parse("2eec5062-c276-4a4e-a31a-f8a1614c1e01"),
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
            Images = CreatePartImages(
                Guid.Parse("2eec5062-c276-4a4e-a31a-f8a1614c1e01"),
                "2eec5062-c276-4a4e-a31a-f8a1614c1e01",
                4),
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
            Id = Guid.Parse("20a4aae5-d07d-4f7d-8cc2-a814700060fe"),
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
            Images = CreatePartImages(
                Guid.Parse("20a4aae5-d07d-4f7d-8cc2-a814700060fe"),
                "20a4aae5-d07d-4f7d-8cc2-a814700060fe",
                4),
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

    private static async Task AttachImagesToExistingSeedDataAsync(CatalogDbContext context)
    {
        var partStorageIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["TOY-OF-001"] = "c585f6bf-8521-4f7b-904b-8dc91b8721d9",
            ["BOS-AF-150"] = "f6df2972-c094-4cab-bfeb-0becdf8ac876",
            ["BRE-PAD-330"] = "44c41429-a0c8-4d9d-8dca-b30b8fcb8556",
            ["KYB-RS-210"] = "59d52775-db46-483b-8f81-619b5e7b80df",
            ["USED-CAMRY-HL-L-01"] = "2eec5062-c276-4a4e-a31a-f8a1614c1e01",
            ["USED-BMW-MIR-R-03"] = "20a4aae5-d07d-4f7d-8cc2-a814700060fe"
        };

        var parts = await context.Parts.Include(x => x.Images).ToListAsync();
        foreach (var part in parts.Where(x => partStorageIds.ContainsKey(x.Article) && x.Images.Count == 0))
        {
            await context.PartImages.AddRangeAsync(
                CreatePartImages(part.Id, partStorageIds[part.Article], 4));
        }

        var carStorageIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Camry"] = "4f41e6ed-023e-47ce-953b-0bbf1c6faf84",
            ["Corolla"] = "9815ff46-c614-4a98-b1e0-7ad7bf5453a2",
            ["X5"] = "dae455c6-22a7-4e90-822d-3796a557bf9a"
        };

        var cars = await context.Cars.Include(x => x.Model).Include(x => x.Images).ToListAsync();
        foreach (var car in cars.Where(x => carStorageIds.ContainsKey(x.Model.Name) && x.Images.Count == 0))
        {
            await context.CarImages.AddRangeAsync(
                CreateCarImages(car.Id, carStorageIds[car.Model.Name], 5));
        }

        await context.SaveChangesAsync();
    }

    private static List<CarImage> CreateCarImages(Guid carId, string storageId, int count) =>
        Enumerable.Range(1, count).Select(index => new CarImage
        {
            Id = Guid.NewGuid(),
            CarId = carId,
            FileKey = $"cars/{storageId}/{index}.jpeg",
            Url = $"https://part-images.storage.yandexcloud.net/cars/{storageId}/{index}.jpeg",
            SortOrder = index - 1,
            IsPrimary = index == 1
        }).ToList();

    private static List<PartImage> CreatePartImages(Guid partId, string storageId, int count) =>
        Enumerable.Range(1, count).Select(index => new PartImage
        {
            Id = Guid.NewGuid(),
            PartId = partId,
            FileKey = $"parts/{storageId}/{index}.jpeg",
            Url = $"https://part-images.storage.yandexcloud.net/parts/{storageId}/{index}.jpeg",
            SortOrder = index - 1,
            IsPrimary = index == 1
        }).ToList();
}
