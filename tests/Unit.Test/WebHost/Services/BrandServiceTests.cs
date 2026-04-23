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
    public class BrandServiceTests
    {
        private readonly Mock<IBrandRepository> _brandRepositoryMock;
        private readonly BrandService _service;

        public BrandServiceTests()
        {
            _brandRepositoryMock = new Mock<IBrandRepository>();
            _service = new BrandService(_brandRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnList()
        {
            // Arrange
            var brands = GenerateCarBrands(3);

            _brandRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(brands);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Select(r => r.Id).Should().BeEquivalentTo(brands.Select(b => b.Id));

            _brandRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetById_ShouldReturnBrand_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var brand = GenerateCarBrand();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(brand.Id);
            result.Name.Should().Be(brand.Name);

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldBeNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarBrand?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldAddBrand()
        {
            // Arrange
            var request = GenerateCreateBrandRequest();

            _brandRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<CarBrand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(request.Name.Trim());
            result.Id.Should().NotBe(Guid.Empty);

            _brandRepositoryMock.Verify(x => x.AddAsync(It.Is<CarBrand>(b => b.Name == request.Name.Trim()),It.IsAny<CancellationToken>()),Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenAlreadyExists()
        {
            // Arrange
            var request = GenerateCreateBrandRequest();
            var existing = GenerateCarBrand();

            var normalizedName = request.Name.Trim();

            _brandRepositoryMock
                .Setup(x => x.GetByNameAsync(normalizedName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Бренд с названием '{normalizedName}' уже существует.");

            _brandRepositoryMock.Verify(x => x.GetByNameAsync(normalizedName, It.IsAny<CancellationToken>()), Times.Once());
            _brandRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CarBrand>(), It.IsAny<CancellationToken>()), Times.Never());

        }


        [Fact]
        public async Task Update_ShouldUpdateBrand_WhenExists()
        {
            // Arrange
            var existingBrand = GenerateCarBrand();
            var updateRequest = GenerateUpdateBrandRequest();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(existingBrand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBrand);

            // Act
            await _service.UpdateAsync(existingBrand.Id, updateRequest);

            // Assert
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(existingBrand.Id), Times.Once);
            _brandRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CarBrand>(b => b.Id == existingBrand.Id)), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GenerateUpdateBrandRequest();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarBrand?)null);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().BeNull();

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _brandRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CarBrand>()), Times.Never);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenAlreadyExists()
        {
            // Arrange
            var request = GenerateUpdateBrandRequest();
            var carBrand = GenerateCarBrand();
            var existing = GenerateCarBrand();

            var normalizedName = request.Name.Trim();
            existing.Name = normalizedName;

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(carBrand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(carBrand);

            _brandRepositoryMock
                .Setup(x => x.GetByNameAsync(normalizedName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            Func<Task> act = () => _service.UpdateAsync(carBrand.Id, request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Бренд с названием '{normalizedName}' уже существует.");

            _brandRepositoryMock.Verify(x => x.GetByNameAsync(normalizedName, It.IsAny<CancellationToken>()), Times.Once());
            _brandRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CarBrand>(), It.IsAny<CancellationToken>()), Times.Never());

        }

        [Fact]
        public async Task Delete_ShouldRemoveBrand_WhenExists()
        {
            // Arrange
            var brand = GenerateCarBrand();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            // Act
            await _service.DeleteAsync(brand.Id);

            // Assert
            _brandRepositoryMock.Verify(x => x.GetByIdAsync(brand.Id), Times.Once);
            _brandRepositoryMock.Verify(x => x.DeleteAsync(brand.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _brandRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarBrand?)null);

            // Act
            Func<Task> act = () => _service.DeleteAsync(id);

            // Assert
            await act.Should().NotThrowAsync();

            _brandRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _brandRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        private List<CarBrand> GenerateCarBrands(int count = 3)
        {
            return new Faker<CarBrand>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate(count);
        }

        private CarBrand GenerateCarBrand()
        {
            return new Faker<CarBrand>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate();
        }

        private CreateBrandRequest GenerateCreateBrandRequest()
        {
            return new Faker<CreateBrandRequest>()
                .RuleFor(r => r.Name, f => $" {f.Company.CompanyName()} ")
                .Generate();
        }

        private UpdateBrandRequest GenerateUpdateBrandRequest()
        {
            return new Faker<UpdateBrandRequest>()
                .RuleFor(r => r.Name, f => $" {f.Company.CompanyName()} Updated ")
                .Generate();
        }
    }
}
