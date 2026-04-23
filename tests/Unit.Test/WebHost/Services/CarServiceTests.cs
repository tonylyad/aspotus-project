using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Enums;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Implementations;
using Bogus;
using FluentAssertions;
using Moq;

namespace Unit.Test.WebHost.Services
{
    public class CarServiceTests
    {
        private readonly Mock<ICarRepository> _carRepositoryMock;
        private readonly Mock<IBrandRepository> _brandRepositoryMock;
        private readonly Mock<ICarModelRepository> _carModelRepositoryMock;
        private readonly Mock<ICarGenerationRepository> _carGenerationRepositoryMock;

        private readonly CarService _service;

        public CarServiceTests()
        {
            _carRepositoryMock = new Mock<ICarRepository>();
            _brandRepositoryMock = new Mock<IBrandRepository>();
            _carModelRepositoryMock = new Mock<ICarModelRepository>();
            _carGenerationRepositoryMock = new Mock<ICarGenerationRepository>();

            _service = new CarService(
                _carRepositoryMock.Object,
                _brandRepositoryMock.Object,
                _carModelRepositoryMock.Object,
                _carGenerationRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetById_ShouldReturnPart_WhenExists()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var generation = GenerateGeneration(model.Id);
            var car = CarWithRelations(GenerateCar(), brand, model, generation);

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            // Act
            var result = await _service.GetByIdAsync(car.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(car.Id);

            _carRepositoryMock.Verify(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Car?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();

            _carRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldCreateCar_WithAllRelations()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var generation = GenerateGeneration(model.Id);
            var request = GenerateCreateCarRequest(brand.Id, model.Id,generation.Id);

            Car? savedCar = null;


            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);

            _carRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
                .Callback<Car, CancellationToken>((c, _) =>
                {
                    c.Brand = brand;
                    c.Model = model;
                    c.Generation = generation;

                    savedCar = c;
                })
                .Returns(Task.CompletedTask);

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => savedCar);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.BrandName.Should().Be(brand.Name);
            result.ModelName.Should().Be(model.Name);
            result.GenerationName.Should().Be(generation.Name);

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()), Times.Once);
            _carRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Create_ShouldThrow_WhenBrandNotFound()
        {
            // Arrange
            var request = GenerateCreateCarRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarBrand?)null);

            // Act
            var act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная марка автомобиля не существует.");

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenModelNotFound()
        {
            // Arrange
            var brand = GenerateBrand();
            var request = GenerateCreateCarRequest(brand.Id, Guid.NewGuid(), Guid.NewGuid());

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            // Act
            var act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная модель автомобиля не существует.");

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenGenerationNotFound()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var generation = GenerateGeneration(model.Id);
            var request = GenerateCreateCarRequest(brand.Id, model.Id, Guid.NewGuid());

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarGeneration?)null);

            // Act
            var act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанное поколение автомобиля не существует.");

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldValidate_ModelBelongsToBrand()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);

            var otherBrand = GenerateBrand();
            var otherModel = GenerateModel(otherBrand.Id);
            var generation = GenerateGeneration(otherModel.Id);

            var request = GenerateCreateCarRequest(brand.Id, model.Id, generation.Id);
            request.BrandId = otherBrand.Id;

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(otherBrand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherBrand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>()
                .Where(ex => ex.Message == "Указанная модель не принадлежит выбранной марке автомобиля.");

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(otherBrand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldValidate_GenerationBelongsToModel()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);

            var otherModel = GenerateModel(brand.Id);
            var otherGeneration = GenerateGeneration(otherModel.Id);

            var request = GenerateCreateCarRequest(brand.Id, model.Id, otherGeneration.Id);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(otherModel.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherModel);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(otherGeneration.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherGeneration);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>()
                .Where(ex => ex.Message == "Указанное поколение не принадлежит выбранной модели автомобиля.");

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(otherGeneration.Id, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Update_ShouldUpdateCar_WithRelations()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var generation = GenerateGeneration(model.Id);

            var car = CarWithRelations(GenerateCar(), brand, model, generation);

            var request = GenerateUpdateCarRequest(brand.Id, model.Id, generation.Id);

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);

            _carRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
                .Callback<Car, CancellationToken>((updated, _) =>
                {
                    car.Brand = brand;
                    car.Model = model;
                    car.Generation = generation;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(car.Id, request);

            // Assert
            result.Should().NotBeNull();
            result.BrandName.Should().Be(brand.Name);
            result.ModelName.Should().Be(model.Name);
            result.GenerationName.Should().Be(generation.Name);

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenCarNotFound()
        {
            // Arrange
            var request = GenerateUpdateCarRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Car?)null);

            // Act
            var result = await _service.UpdateAsync(Guid.NewGuid(), request);

            // Assert
            result.Should().BeNull();

            _carRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenBrandNotFound()
        {
            // Arrange
            var car = GenerateCar();
            var request = GenerateUpdateCarRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarBrand?)null);

            // Act
            var act = async () => await _service.UpdateAsync(car.Id, request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная марка автомобиля не существует.");

            _carRepositoryMock.Verify(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenModelNotFound()
        {
            // Arrange
            var car = GenerateCar();
            var brand = GenerateBrand();
            var request = GenerateUpdateCarRequest(brand.Id, Guid.NewGuid(), Guid.NewGuid());

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            // Act
            var act = async () => await _service.UpdateAsync(car.Id, request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная модель автомобиля не существует.");

            _carRepositoryMock.Verify(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenGenerationlNotFound()
        {
            // Arrange
            var car = GenerateCar();
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);
            var request = GenerateUpdateCarRequest(brand.Id, model.Id, Guid.NewGuid());

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarGeneration?)null);

            // Act
            var act = async () => await _service.UpdateAsync(car.Id, request);

            //Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанное поколение автомобиля не существует.");

            _carRepositoryMock.Verify(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldValidate_ModelBelongsToBrand()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);

            var otherBrand = GenerateBrand();
            var otherModel = GenerateModel(otherBrand.Id);
            var generation = GenerateGeneration(otherModel.Id);

            var car = GenerateCar();

            var request = GenerateUpdateCarRequest(otherBrand.Id, model.Id, generation.Id);

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(otherBrand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherBrand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(generation);

            // Act
            Func<Task> act = () => _service.UpdateAsync(car.Id,request);

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>()
                .Where(ex => ex.Message == "Указанная модель не принадлежит выбранной марке автомобиля.");

            _carRepositoryMock.Verify(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(otherBrand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(generation.Id, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Update_ShouldValidate_GenerationBelongsToModel()
        {
            // Arrange
            var brand = GenerateBrand();
            var model = GenerateModel(brand.Id);

            var otherModel = GenerateModel(brand.Id);
            var otherGeneration = GenerateGeneration(otherModel.Id);

            var car = GenerateCar();

            var request = GenerateUpdateCarRequest(brand.Id, model.Id, otherGeneration.Id);

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(otherModel.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherModel);

            _carGenerationRepositoryMock
                .Setup(x => x.GetByIdAsync(otherGeneration.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherGeneration);

            // Act
            Func<Task> act = () => _service.UpdateAsync(car.Id,request);

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>()
                .Where(ex => ex.Message == "Указанное поколение не принадлежит выбранной модели автомобиля.");

            _carRepositoryMock.Verify(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carGenerationRepositoryMock.Verify(x => x.GetByIdAsync(otherGeneration.Id, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Delete_ShouldRemoveCar_WhenExists()
        {
            // Arrange
            var car = GenerateCar();

            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _carRepositoryMock
                .Setup(x => x.DeleteAsync(car.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(car.Id);

            // Assert
            result.Should().BeTrue();

            _carRepositoryMock.Verify(x => x.GetByIdAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carRepositoryMock.Verify(x => x.DeleteAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            // Arrange
            _carRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Car?)null);

            // Act
            var result = await _service.DeleteAsync(Guid.NewGuid());

            // Assert
            result.Should().BeFalse();

            _carRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
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
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Company.CompanyName())
                .RuleFor(g => g.ModelId, f => modelId)
                .Generate();
        }

        private PartCompatibility GeneratePartCompatibility()
        {
            return new Faker<PartCompatibility>()
                .RuleFor(p => p.PartId, f => f.Random.Guid())
                .RuleFor(p => p.CarId, f => f.Random.Guid())
                .Generate();
        }

        private Car GenerateCar(
            Guid? brandId = null,
            Guid? modelId = null,
            Guid? generationId = null,
            CarBrand? brand = null,
            CarModel? model = null,
            CarGeneration? generation = null)
        {
            brand ??= GenerateBrand();
            model ??= GenerateModel(brand.Id);
            generation ??= GenerateGeneration(model.Id);

            return new Faker<Car>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.BrandId, f => brandId ?? brand.Id)
                .RuleFor(x => x.ModelId, f => modelId ?? model.Id)
                .RuleFor(x => x.GenerationId, f => generationId ?? generation.Id)

                .RuleFor(x => x.Brand, f => brand)
                .RuleFor(x => x.Model, f => model)
                .RuleFor(x => x.Generation, f => generation)

                .RuleFor(c => c.Year, f => f.Date.Between(new System.DateTime(1980, 1, 1), new System.DateTime(2024, 12, 31)).Year)

                .RuleFor(c => c.Mileage, f => f.Random.Int(0, 300000))

                .RuleFor(c => c.BodyType, f => f.PickRandom(new[]
                {
                    "Sedan", "SUV", "Hatchback", "Coupe", "Convertible",
                    "Wagon", "Minivan", "Pickup", "Crossover", "Roadster"
                }))

                .RuleFor(c => c.TrimLevelName, f =>
                {
                    if (f.Random.Bool(0.7f))
                    {
                        return f.Commerce.ProductName();
                    }
                    return null;
                })

                .RuleFor(c => c.TrimLevelDescription, (f, c) =>
                {
                    if (!string.IsNullOrEmpty(c.TrimLevelName) && f.Random.Bool(0.8f))
                    {
                        return $"{f.Lorem.Sentence()} Includes: {f.Commerce.ProductAdjective()}, {f.Commerce.ProductAdjective()} features";
                    }
                    return null;
                })

                .RuleFor(c => c.EngineVolume, f =>
                {
                    var volumes = new decimal[] { 1.0m, 1.2m, 1.4m, 1.6m, 1.8m, 2.0m, 2.2m, 2.4m, 2.5m, 3.0m, 3.5m, 4.0m, 5.0m };
                    return volumes[f.Random.Number(0, volumes.Length - 1)];
                })

                .RuleFor(c => c.FuelType, f => f.PickRandom(new[]
                {
                    "Petrol", "Diesel", "Hybrid", "Electric", "Plug‑in Hybrid", "CNG", "LPG", "Hydrogen"
                }))

                .RuleFor(c => c.TransmissionType, f => f.PickRandom(new[]
                {
                    "Manual", "Automatic", "Variator", "DCT", "AMT", "Sequential"
                }))

                .RuleFor(c => c.DriveType, f => f.PickRandom(new[]
                {
                "FWD", "RWD", "AWD", "4WD", "Rear‑Wheel Drive", "Front‑Wheel Drive"
                }))

                .RuleFor(c => c.PartCompatibilities, f =>
                        {
                            var count = f.Random.Number(0, 5);
                            var compatibilities = new List<PartCompatibility>();
                            for (int i = 0; i < count; i++)
                            {
                                compatibilities.Add(GeneratePartCompatibility());
                            }
                            return compatibilities;
                        })

                .Generate();
        }

        private Car CarWithRelations(Car car, CarBrand brand, CarModel model, CarGeneration generation)
        {
            car.Brand = brand;
            car.Model = model;
            car.Generation = generation;
            return car;

        }

        private CreateCarRequest GenerateCreateCarRequest(Guid brandId, Guid modelId, Guid generationId)
        {
            return new Faker<CreateCarRequest>()
                .RuleFor(x => x.BrandId, brandId)
                .RuleFor(x => x.ModelId, modelId)
                .RuleFor(x => x.GenerationId, generationId)

                .RuleFor(c => c.Year, f => f.Date.Between(new System.DateTime(1980, 1, 1), new System.DateTime(2024, 12, 31)).Year)

                .RuleFor(c => c.Mileage, f => f.Random.Int(0, 300000))

                .RuleFor(c => c.BodyType, f => f.PickRandom(new[]
                {
                    "Sedan", "SUV", "Hatchback", "Coupe", "Convertible",
                    "Wagon", "Minivan", "Pickup", "Crossover", "Roadster"
                }))

                .RuleFor(c => c.TrimLevelName, f =>
                {
                    if (f.Random.Bool(0.7f))
                    {
                        return f.Commerce.ProductName();
                    }
                    return null;
                })

                .RuleFor(c => c.TrimLevelDescription, (f, c) =>
                {
                    if (!string.IsNullOrEmpty(c.TrimLevelName) && f.Random.Bool(0.8f))
                    {
                        return $"{f.Lorem.Sentence()} Includes: {f.Commerce.ProductAdjective()}, {f.Commerce.ProductAdjective()} features";
                    }
                    return null;
                })

                .RuleFor(c => c.EngineVolume, f =>
                {
                    var volumes = new decimal[] { 1.0m, 1.2m, 1.4m, 1.6m, 1.8m, 2.0m, 2.2m, 2.4m, 2.5m, 3.0m, 3.5m, 4.0m, 5.0m };
                    return volumes[f.Random.Number(0, volumes.Length - 1)];
                })

                .RuleFor(c => c.FuelType, f => f.PickRandom(new[]
                {
                    "Petrol", "Diesel", "Hybrid", "Electric", "Plug‑in Hybrid", "CNG", "LPG", "Hydrogen"
                }))

                .RuleFor(c => c.TransmissionType, f => f.PickRandom(new[]
                {
                    "Manual", "Automatic", "Variator", "DCT", "AMT", "Sequential"
                }))

                .RuleFor(c => c.DriveType, f => f.PickRandom(new[]
                {
                "FWD", "RWD", "AWD", "4WD", "Rear‑Wheel Drive", "Front‑Wheel Drive"
                }))

                .Generate();
        }

        private UpdateCarRequest GenerateUpdateCarRequest(Guid brandId, Guid modelId, Guid generationId)
        {
            return new Faker<UpdateCarRequest>()
                .RuleFor(x => x.BrandId, brandId)
                .RuleFor(x => x.ModelId, modelId)
                .RuleFor(x => x.GenerationId, generationId)

                .RuleFor(c => c.Year, f => f.Date.Between(new System.DateTime(1980, 1, 1), new System.DateTime(2024, 12, 31)).Year)

                .RuleFor(c => c.Mileage, f => f.Random.Int(0, 300000))

                .RuleFor(c => c.BodyType, f => f.PickRandom(new[]
                {
                    "Sedan", "SUV", "Hatchback", "Coupe", "Convertible",
                    "Wagon", "Minivan", "Pickup", "Crossover", "Roadster"
                }))

                .RuleFor(c => c.TrimLevelName, f =>
                {
                    if (f.Random.Bool(0.7f))
                    {
                        return f.Commerce.ProductName();
                    }
                    return null;
                })

                .RuleFor(c => c.TrimLevelDescription, (f, c) =>
                {
                    if (!string.IsNullOrEmpty(c.TrimLevelName) && f.Random.Bool(0.8f))
                    {
                        return $"{f.Lorem.Sentence()} Includes: {f.Commerce.ProductAdjective()}, {f.Commerce.ProductAdjective()} features";
                    }
                    return null;
                })

                .RuleFor(c => c.EngineVolume, f =>
                {
                    var volumes = new decimal[] { 1.0m, 1.2m, 1.4m, 1.6m, 1.8m, 2.0m, 2.2m, 2.4m, 2.5m, 3.0m, 3.5m, 4.0m, 5.0m };
                    return volumes[f.Random.Number(0, volumes.Length - 1)];
                })

                .RuleFor(c => c.FuelType, f => f.PickRandom(new[]
                {
                    "Petrol", "Diesel", "Hybrid", "Electric", "Plug‑in Hybrid", "CNG", "LPG", "Hydrogen"
                }))

                .RuleFor(c => c.TransmissionType, f => f.PickRandom(new[]
                {
                    "Manual", "Automatic", "Variator", "DCT", "AMT", "Sequential"
                }))

                .RuleFor(c => c.DriveType, f => f.PickRandom(new[]
                {
                "FWD", "RWD", "AWD", "4WD", "Rear‑Wheel Drive", "Front‑Wheel Drive"
                }))

                .Generate();
        }
    }
}
