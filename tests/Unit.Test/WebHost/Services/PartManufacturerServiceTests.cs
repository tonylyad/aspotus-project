
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

    public class PartManufacturerServiceTest
    {
        private readonly Mock<IPartManufacturerRepository> _partManufacturerRepositoryMock;
        private readonly PartManufacturerService _service; 

        public PartManufacturerServiceTest()
        {
            _partManufacturerRepositoryMock = new Mock<IPartManufacturerRepository>();

            _service = new PartManufacturerService(_partManufacturerRepositoryMock.Object);

        }

        [Fact]
        public async Task GetById_ShouldReturnManufacturer_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var partManufacturer = GeneratePartManufacturer();

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partManufacturer);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(partManufacturer.Id);
            result.Name.Should().Be(partManufacturer.Name);

            _partManufacturerRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldBeNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartManufacturer?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();

            _partManufacturerRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnList()
        {
            // Arrange
            var partManufacturers = GeneratePartManufactures(3);

            _partManufacturerRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(partManufacturers);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Select(r => r.Id).Should().BeEquivalentTo(partManufacturers.Select(b => b.Id));

            _partManufacturerRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldTrimAndAddManufacturer()
        {
            // Arrange
            var request = GenerateCreatePartManufacturerRequest();

            _partManufacturerRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<PartManufacturer>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(request.Name.Trim());
            result.Id.Should().NotBe(Guid.Empty);

            _partManufacturerRepositoryMock.Verify(x => x.AddAsync(It.Is<PartManufacturer>(b => b.Name == request.Name.Trim()), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenAlreadyExists()
        {
            // Arrange
            var request = GenerateCreatePartManufacturerRequest();
            var existing = GeneratePartManufacturer();

            var normalizedName = request.Name.Trim();

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByNameAsync(normalizedName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Производитель с названием '{normalizedName}' уже существует.");

            _partManufacturerRepositoryMock.Verify(x => x.GetByNameAsync(normalizedName, It.IsAny<CancellationToken>()), Times.Once());
            _partManufacturerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PartManufacturer>(), It.IsAny<CancellationToken>()), Times.Never());

        }

        [Fact]
        public async Task Update_ShouldUpdateBrand_WhenExists()
        {
            // Arrange
            var existingManufacturer = GeneratePartManufacturer();
            var updateRequest = GenerateUpdatePartManufacturerRequest();

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(existingManufacturer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingManufacturer);

            // Act
            await _service.UpdateAsync(existingManufacturer.Id, updateRequest);

            // Assert
            _partManufacturerRepositoryMock.Verify(x => x.GetByIdAsync(existingManufacturer.Id), Times.Once);
            _partManufacturerRepositoryMock.Verify(x => x.UpdateAsync(It.Is<PartManufacturer>(b => b.Id == existingManufacturer.Id)), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GenerateUpdatePartManufacturerRequest();

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartManufacturer?)null);

            // Act
            Func<Task> act = () => _service.UpdateAsync(id, request);

            // Assert
            await act.Should().NotThrowAsync();

            _partManufacturerRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _partManufacturerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<PartManufacturer>()), Times.Never);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenAlreadyExists()
        {
            // Arrange
            var request = GenerateUpdatePartManufacturerRequest();
            var partManufacturer = GeneratePartManufacturer();
            var existing = GeneratePartManufacturer();

            var normalizedName = request.Name.Trim();
            existing.Name = normalizedName;

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(partManufacturer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partManufacturer);

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByNameAsync(normalizedName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            Func<Task> act = () => _service.UpdateAsync(partManufacturer.Id, request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Производитель с названием '{normalizedName}' уже существует.");

            _partManufacturerRepositoryMock.Verify(x => x.GetByNameAsync(normalizedName, It.IsAny<CancellationToken>()), Times.Once());
            _partManufacturerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<PartManufacturer>(), It.IsAny<CancellationToken>()), Times.Never());

        }

        [Fact]
        public async Task Delete_ShouldRemoveBrand_WhenExists()
        {
            // Arrange
            var partManufacturer = GeneratePartManufacturer();

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(partManufacturer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partManufacturer);

            // Act
            await _service.DeleteAsync(partManufacturer.Id);

            // Assert
            _partManufacturerRepositoryMock.Verify(x => x.GetByIdAsync(partManufacturer.Id), Times.Once);
            _partManufacturerRepositoryMock.Verify(x => x.DeleteAsync(partManufacturer.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _partManufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartManufacturer?)null);

            // Act
            Func<Task> act = () => _service.DeleteAsync(id);

            // Assert
            await act.Should().NotThrowAsync();

            _partManufacturerRepositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _partManufacturerRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        private List<PartManufacturer> GeneratePartManufactures(int count = 3)
        {
            return new Faker<PartManufacturer>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate(count);
        }

        private PartManufacturer GeneratePartManufacturer()
        {
            return new Faker<PartManufacturer>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate();
        }

        private CreatePartManufacturerRequest GenerateCreatePartManufacturerRequest()
        {
            return new Faker<CreatePartManufacturerRequest>()
                .RuleFor(r => r.Name, f => $" {f.Company.CompanyName()} ")
                .Generate();
        }

        private UpdatePartManufacturerRequest GenerateUpdatePartManufacturerRequest()
        {
            return new Faker<UpdatePartManufacturerRequest>()
                .RuleFor(r => r.Name, f => $" {f.Company.CompanyName()} Updated ")
                .Generate();
        }

    }

}
