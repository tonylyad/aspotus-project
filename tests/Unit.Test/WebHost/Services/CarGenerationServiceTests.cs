using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Implementations;
using AwesomeAssertions;
using Bogus;
using Moq;


namespace Unit.Test.WebHost.Services
{
    public class CarGenerationServiceTests
    {
        private readonly Mock<ICarGenerationRepository> _carGenerationRepositoryMock;
        private readonly Mock<ICarModelRepository> _carModelRepositoryMock;

        public readonly CarGenerationService _service;

        public CarGenerationServiceTests()
        {
            _carGenerationRepositoryMock = new Mock<ICarGenerationRepository>();
            _carModelRepositoryMock = new Mock<ICarModelRepository>();

            _service = new CarGenerationService(_carGenerationRepositoryMock.Object, _carModelRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnGeneration_WhenExists()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var carGeneration = GenerationWithRelations(GenerateGeneration(model.Id), brand, model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(carGeneration.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(carGeneration);

            // Act
            var result = await _service.GetByIdAsync(carGeneration.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(carGeneration.Id);

            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(carGeneration.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarGeneration?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();

            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldCreateCarGeneration_WithAllRelations()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var request = GenerateCreateGenerationRequest(model.Id);

            CarGeneration? savedCarGeneration = null;

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<CarGeneration>(), It.IsAny<CancellationToken>()))
                .Callback<CarGeneration, CancellationToken>((c, _) =>
                {
                    c.Model = model;

                    savedCarGeneration = c;
                })
                .Returns(Task.CompletedTask);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => savedCarGeneration);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.ModelName.Should().Be(model.Name);

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CarGeneration>(), It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Create_ShouldThrow_WhenModelNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GenerateCreateGenerationRequest(id);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            // Act
            var act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная модель автомобиля не существует.");

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenGenerationExists()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);

            var request = GenerateCreateGenerationRequest(model.Id);

            var existingGeneration = GenerateGeneration(model.Id);
            existingGeneration.Name = request.Name;

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByNameAsync(existingGeneration.Name, model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingGeneration);

            // Act
            var act = async () => await _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Поколение '{existingGeneration.Name}' уже существует у выбранной модели.");

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByNameAsync(existingGeneration.Name, model.Id, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Update_ShouldUpdateGeneration_WithRelations()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var generation = GenerationWithRelations(GenerateGeneration(model.Id),brand,model);

            var request = GenerateUpdateGenerationRequest(model.Id);

            CarGeneration? updatedGeneration = null;

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<CarGeneration>(), It.IsAny<CancellationToken>()))
                .Callback<CarGeneration, CancellationToken>((updated, _) =>
                {
                    updated.Model = model;
                    updatedGeneration = updated;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(generation.Id, request);

            // Assert
            result.Should().NotBeNull();
            result.ModelName.Should().Be(model.Name);

            _carGenerationRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CarGeneration>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenCarGenerationNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GenerateUpdateGenerationRequest(Guid.NewGuid());

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarGeneration?)null);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().BeNull();

            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenModelNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var generation = GenerateGeneration(model.Id);
            var request = GenerateUpdateGenerationRequest(Guid.NewGuid());

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            // Act
            var act = () => _service.UpdateAsync(generation.Id, request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная модель автомобиля не существует.");

            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenGenerationExists()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var generation = GenerateGeneration(model.Id);

            var request = GenerateUpdateGenerationRequest(model.Id);

            var existingGeneration = GenerateGeneration(model.Id);
            existingGeneration.Name = request.Name;
            
            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByNameAsync(existingGeneration.Name, model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingGeneration);

            // Act
            var act = async () => await _service.UpdateAsync(generation.Id,request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Поколение '{existingGeneration.Name}' уже существует у выбранной модели.");

            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByNameAsync(existingGeneration.Name, model.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldRemoveCar_WhenExists()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var carGeneration = GenerateGeneration(model.Id);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(carGeneration.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(carGeneration);

            _carGenerationRepositoryMock
                .Setup(x => x.DeleteAsync(carGeneration.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(carGeneration.Id);

            // Assert
            result.Should().BeTrue();

            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(carGeneration.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.DeleteAsync(carGeneration.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarGeneration?)null);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeFalse();

            _carGenerationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByModelIdAsync_WhenGenerationsExist_ShouldReturnCollection()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            model.Brand = brand;
            List<CarGeneration> generations = new List<CarGeneration>();

            for (int i = 0; i<3; i++)
            {
                var gen = GenerateGeneration(model.Id);

                gen = GenerationWithRelations(gen, brand, model);

                generations.Add(gen);
            }

            _carModelRepositoryMock
                .Setup(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(r => r.GetByModelIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(generations);

            // Act
            var result = await _service.GetByModelIdAsync(model.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(generations.Count);

            var expectedNames = generations.Select(g => g.Name).OrderBy(n => n).ToList();
            var actualNames = result.Select(r => r.Name).OrderBy(n => n).ToList();

            actualNames.Should().BeEquivalentTo(expectedNames);

            _carModelRepositoryMock.Verify(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()),Times.Once);
            _carGenerationRepositoryMock.Verify(r => r.GetByModelIdAsync(model.Id, It.IsAny<CancellationToken>()),Times.Once);
        }


        [Fact]
        public async Task GetByModelIdAsync_WhenNoGenerations_ShouldReturnEmptyCollection()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);

            _carModelRepositoryMock
                .Setup(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(r => r.GetByModelIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CarGeneration>());

            // Act
            var result = await _service.GetByModelIdAsync(model.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(0, "Коллекция должна быть пустой");

            _carModelRepositoryMock.Verify(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(r => r.GetByModelIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        private CarBrand GenerateBrand()
        {
            return new Faker<CarBrand>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Commerce.Categories(1)[0])
                .Generate();
        }

        private CarModel GenerateModel(Guid brandId)
        {
            return new Faker<CarModel>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Company.CompanyName())
                .RuleFor(m => m.BrandId, f => brandId)
                .Generate();
        }

        private CarGeneration GenerateGeneration(Guid modelId)
        {
            return new Faker<CarGeneration>()
                .RuleFor(g => g.Id, f => f.Random.Guid())
                .RuleFor(g => g.Name, f =>
                {
                    var prefixes = new[] { "XV", "E", "G", "F", "C", "T", "S" };
                    var numbers = new[] { f.Random.Number(10, 99), f.Random.Number(100, 999) };
                    return $"{f.PickRandom(prefixes)}{f.PickRandom(numbers)}";
                })
                .RuleFor(g => g.YearFrom, f => f.Date.Past(20).Year)
                .RuleFor(g => g.YearTo, (f, g) =>
                {
                    if (f.Random.Bool(0.3f))
                    {
                        return null;
                    }
                    var endYear = g.YearFrom + f.Random.Number(3, 15);
                    return endYear <= DateTime.Now.Year ? endYear : (int?)null;
                })
                .RuleFor(g => g.ModelId, f => modelId)

                .RuleFor(g => g.Cars, f => new List<Car>())

                .Generate();
        }

        private CarGeneration GenerationWithRelations(CarGeneration carGeneration, CarBrand brand, CarModel model)
        {
            model.Brand = brand;
            carGeneration.Model = model;
            return carGeneration;
        }

        private CreateCarGenerationRequest GenerateCreateGenerationRequest(Guid modelId)
        {
            return new Faker<CreateCarGenerationRequest>()
               .RuleFor(r => r.Name, f =>
                        {
                            var prefixes = new[] { "XV", "E", "G", "F", "C", "T", "S" };
                            var numbers = new[] { f.Random.Number(10, 99), f.Random.Number(100, 999) };
                            return $"{f.PickRandom(prefixes)}{f.PickRandom(numbers)}";
                        })
            .RuleFor(r => r.YearFrom, f => f.Date.Past(20).Year)
            .RuleFor(r => r.YearTo, (f, r) =>
            {
                if (f.Random.Bool(0.3f)) // 30 % вероятность null (всё ещё выпускается)
                {
                    return null;
                }
                var endYear = r.YearFrom + f.Random.Number(3, 15);
                return endYear <= DateTime.Now.Year ? (int?)endYear : (int?)null;
            })
            .RuleFor(r => r.ModelId, modelId)
            .Generate();
        }

        private UpdateCarGenerationRequest GenerateUpdateGenerationRequest(Guid modelId)
        {
            return new Faker<UpdateCarGenerationRequest>()
               .RuleFor(r => r.Name, f =>
               {
                   var prefixes = new[] { "XV", "E", "G", "F", "C", "T", "S" };
                   var numbers = new[] { f.Random.Number(10, 99), f.Random.Number(100, 999) };
                   return $"{f.PickRandom(prefixes)}{f.PickRandom(numbers)}";
               })
            .RuleFor(r => r.YearFrom, f => f.Date.Past(20).Year)
            .RuleFor(r => r.YearTo, (f, r) =>
            {
                if (f.Random.Bool(0.3f)) // 30 % вероятность null (всё ещё выпускается)
                {
                    return null;
                }
                var endYear = r.YearFrom + f.Random.Number(3, 15);
                return endYear <= DateTime.Now.Year ? (int?)endYear : (int?)null;
            })
            .RuleFor(r => r.ModelId, modelId)
            .Generate();
        }
    }
}
