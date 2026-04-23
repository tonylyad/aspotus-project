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
    public class CarModelServiceTests
    {
        private readonly Mock<ICarModelRepository> _carModelRepositoryMock;
        private readonly Mock<IBrandRepository> _brandRepositoryMock;
        private readonly CarModelService _service;

        public CarModelServiceTests()
        {
            _carModelRepositoryMock = new Mock<ICarModelRepository>();
            _brandRepositoryMock = new Mock<IBrandRepository>();

            _service = new CarModelService(_carModelRepositoryMock.Object, _brandRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnModel_WhenExists()
        {
            // Arrange
            var brand = GenerateCarBrand();
            var model = GenerateCarModel(brand.Id);

            model.Brand = brand;

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            // Act
            var result = await _service.GetByIdAsync(model.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(model.Id);
            result.Name.Should().Be(model.Name);
            result.BrandId.Should().Be(brand.Id);
            result.BrandName.Should().Be(brand.Name);

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetAll_ShouldReturnList()
        {
            // Arrange
            var carModels = GenerateCarModels(3);

            _carModelRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(carModels);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Select(r => r.Id).Should().BeEquivalentTo(carModels.Select(b => b.Id));

            _carModelRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldCreateModel()
        {
            // Arrange
            var brand = GenerateCarBrand();
            var request = GenerateCreateCarModelRequest(brand.Id);

            var normalizedName = request.Name.Trim();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByNameAsync(normalizedName, brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            CarModel? savedModel = null;

            _carModelRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()))
                .Callback<CarModel, CancellationToken>((m, _) =>
                {
                    m.Brand = brand;
                    savedModel = m;
                })
                .Returns(Task.CompletedTask);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => savedModel);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(normalizedName);

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once());
            _carModelRepositoryMock.Verify(x => x.GetByNameAsync(normalizedName, brand.Id, It.IsAny<CancellationToken>()), Times.Once());
            _carModelRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenAlreadyExists()
        {
            // Arrange
            var brand = GenerateCarBrand();
            var request = GenerateCreateCarModelRequest(brand.Id);
            var existing = GenerateCarModel(brand.Id);

            var normalizedName = request.Name.Trim();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByNameAsync(normalizedName, request.BrandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Модель '{normalizedName}' уже существует у выбранной марки.");

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()),Times.Once());
            _carModelRepositoryMock.Verify(x => x.GetByNameAsync(normalizedName, brand.Id, It.IsAny<CancellationToken>()),Times.Once());
            _carModelRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()),Times.Never());
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()),Times.AtMostOnce());
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenBrandNotFound()
        {
            // Arrange
            var request = GenerateCreateCarModelRequest(Guid.NewGuid());

            var normalizedName = request.Name.Trim();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(request.BrandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarBrand?)null);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная марка автомобиля не существует.");

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(request.BrandId, It.IsAny<CancellationToken>()), Times.Once());
            _carModelRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Update_ShouldUpdateModel()
        {
            // Arrange
            var brand = GenerateCarBrand();
            var model = GenerateCarModel(brand.Id);
            var request = GenerateUpdateCarModelRequest(brand.Id);

            var normalizedName = request.Name.Trim();

            var updatedModel = new CarModel
            {
                Id = model.Id,
                Name = normalizedName,
                BrandId = brand.Id,
                Brand = brand,
            };

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carModelRepositoryMock
                .Setup(x => x.GetByNameAsync(normalizedName,brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()))
                .Callback<CarModel, CancellationToken>((c, _) =>
                {
                    c.Name = normalizedName;
                    c.BrandId = brand.Id;
                    c.Brand = brand;
                })
                .Returns(Task.CompletedTask);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedModel);

            // Act
            var result = await _service.UpdateAsync(model.Id,request);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(normalizedName);
            result.BrandId.Should().Be(brand.Id);

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Exactly(2));
            _carModelRepositoryMock.Verify(x => x.GetByNameAsync(normalizedName, brand.Id, It.IsAny<CancellationToken>()), Times.Once());
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once());
            _carModelRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Update_ShoulReturnNull_WhenModelNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GenerateUpdateCarModelRequest(Guid.NewGuid());

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().BeNull();

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenAlreadyExists()
        {
            // Arrange
            var brand = GenerateCarBrand();
            var model = GenerateCarModel(brand.Id);
            var request = GenerateUpdateCarModelRequest(brand.Id);
            var existing = GenerateCarModel(brand.Id);

            var normalizedName = request.Name.Trim();

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(x => x.GetByNameAsync(normalizedName, request.BrandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            Func<Task> act = () => _service.UpdateAsync(model.Id,request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Модель '{normalizedName}' уже существует у выбранной марки.");

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once());
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once());
            _carModelRepositoryMock.Verify(x => x.GetByNameAsync(normalizedName, brand.Id, It.IsAny<CancellationToken>()), Times.Once());
            _carModelRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()), Times.Never());
            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()), Times.AtMostOnce());
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenBrandNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GenerateUpdateCarModelRequest(id);
            var model = GenerateCarModel(id);

            var normalizedName = request.Name.Trim();

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carModelRepositoryMock
                .Setup(x => x.GetByNameAsync(It.IsAny<string>(),id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(request.BrandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarBrand?)null);

            // Act
            Func<Task> act = () => _service.UpdateAsync(model.Id,request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная марка автомобиля не существует.");

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once());
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(request.BrandId, It.IsAny<CancellationToken>()), Times.Once());
            _carModelRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Delete_ShouldRemoveCar_WhenExists()
        {
            // Arrange
            var brand = GenerateCarBrand();
            var model = GenerateCarModel(brand.Id);

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);

            _carModelRepositoryMock
                .Setup(x => x.DeleteAsync(model.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(model.Id);

            // Assert
            result.Should().BeTrue();

            _carModelRepositoryMock.Verify(x => x.GetByIdAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(x => x.DeleteAsync(model.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _carModelRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarModel?)null);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeFalse();

            _carModelRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByBrandIdAsync_WhenBrandExist_ShouldReturnCollection()
        {
            // Arrange
            var brand = GenerateCarBrand();
            List<CarModel> carModels = new List<CarModel>();

            for (int i = 0; i < 3; i++)
            {
                var cm = GenerateCarModel(brand.Id);
                cm.Brand = brand;
                carModels.Add(cm);
            }

            _brandRepositoryMock
                .Setup(r => r.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(r => r.GetByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(carModels);

            // Act
            var result = await _service.GetByBrandIdAsync(brand.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(carModels.Count);

            var expectedNames = carModels.Select(g => g.Name).OrderBy(n => n).ToList();
            var actualNames = result.Select(r => r.Name).OrderBy(n => n).ToList();

            actualNames.Should().BeEquivalentTo(expectedNames);

            foreach (var response in result)
            {
                response.BrandId.Should().Be(brand.Id);
                response.BrandName.Should().Be(brand.Name);
            }

            _brandRepositoryMock.Verify(r => r.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            _carModelRepositoryMock.Verify(r => r.GetByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByModelIdAsync_WhenNoGenerations_ShouldReturnEmptyCollection()
        {
            // Arrange
            var brand = GenerateCarBrand();

            _brandRepositoryMock
                .Setup(r => r.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            _carModelRepositoryMock
                .Setup(r => r.GetByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CarModel>());

            // Acts
            var result = await _service.GetByBrandIdAsync(brand.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(0);

            _carModelRepositoryMock.Verify(r => r.GetByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        private CarBrand GenerateCarBrand()
        {
            return new Faker<CarBrand>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate();
        }

        private List<CarModel> GenerateCarModels(int count = 3)
        {
            return new Faker<CarModel>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .RuleFor(b=> b.Brand, GenerateCarBrand())
                .Generate(count);
        }

        private CarModel GenerateCarModel(Guid id)
        {
            return new Faker<CarModel>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .RuleFor(b => b.BrandId, id)
                .Generate();
        }

        private CreateCarModelRequest GenerateCreateCarModelRequest(Guid id)
        {
            return new Faker<CreateCarModelRequest>()
                .RuleFor(r => r.Name, f => $" {f.Vehicle.Model()}")
                .RuleFor(r => r.BrandId, id)
                .Generate();
        }

        private UpdateCarModelRequest GenerateUpdateCarModelRequest(Guid id)
        {
            return new Faker<UpdateCarModelRequest>()
                .RuleFor(r => r.Name, f => f.Vehicle.Model())
                .RuleFor(r => r.BrandId, id)
                .Generate();
        }
    }
}
