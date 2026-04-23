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
    public class PartCategoryServiceTests
    {
        private readonly Mock<IPartCategoryRepository> _partCategoryrepository;
        private readonly PartCategoryService _service;

        public PartCategoryServiceTests()
        {
            _partCategoryrepository = new Mock<IPartCategoryRepository>();
            _service = new PartCategoryService(_partCategoryrepository.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnCategory_WhenExists()
        {
            // Arrange
            var category = GeneratePartCategory();

            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            // Act
            var result = await _service.GetByIdAsync(category.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(category.Id);
            result.Name.Should().Be(category.Name);

            _partCategoryrepository.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategory?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();

            _partCategoryrepository.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Create_ShouldCreateCategory()
        {
            // Arrange
            var request = GenerateCreatePartCategoryRequest();

            _partCategoryrepository
                .Setup(x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategory?)null);

            _partCategoryrepository
                .Setup(x => x.AddAsync(It.IsAny<PartCategory>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(request.Name);

            _partCategoryrepository.Verify(x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()), Times.Once());
            _partCategoryrepository.Verify(x => x.AddAsync(It.IsAny<PartCategory>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenAlreadyExists()
        {
            // Arrange
            var request = GenerateCreatePartCategoryRequest();
            var existing = GeneratePartCategory();

            _partCategoryrepository
                .Setup(x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Категория с названием '{request.Name}' уже существует.");

            _partCategoryrepository.Verify(x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenParentCategoryNotFound()
        {
            // Arrange
            var request = GenerateCreatePartCategoryRequest();
            request.ParentCategoryId = Guid.NewGuid();

            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(request.ParentCategoryId.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategory?)null);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _partCategoryrepository.Verify(x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Update_ShouldUpdateCategory()
        {
            // Arrange
            var category = GeneratePartCategory();
            var request = GenerateUpdateRequest();

            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _partCategoryrepository
                .Setup(x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategory?)null);

            _partCategoryrepository
                .Setup(x => x.UpdateAsync(It.IsAny<PartCategory>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(category.Id, request);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(request.Name);

            _partCategoryrepository.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once());
            _partCategoryrepository.Verify(x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()), Times.Once());
            _partCategoryrepository.Verify(x => x.UpdateAsync(It.IsAny<PartCategory>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Update_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GenerateUpdateRequest();

            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategory?)null);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().BeNull();

            _partCategoryrepository.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenNameExists()
        {
            // Arrange
            var category = GeneratePartCategory();
            var existing = GeneratePartCategory();
            var request = GenerateUpdateRequest();

            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _partCategoryrepository
                .Setup(x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            Func<Task> act = () => _service.UpdateAsync(category.Id, request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Категория с названием '{request.Name}' уже существует.");

            _partCategoryrepository.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once());
            _partCategoryrepository.Verify( x => x.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenParentCategoryIsParentToItself()
        {
            // Arrange
            var category = GeneratePartCategory();
            category.ParentCategoryId = Guid.NewGuid();
            var request = GenerateUpdateRequest();
            request.ParentCategoryId = category.Id;

            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            // Act
            Func<Task> act = () => _service.UpdateAsync(category.Id, request);

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>();

            _partCategoryrepository.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once());
            _partCategoryrepository.Verify(x => x.UpdateAsync(It.IsAny<PartCategory>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Delete_ShouldDelete_WhenExists()
        {
            // Arrange
            var category = GeneratePartCategory();

            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _partCategoryrepository
                .Setup(x => x.DeleteAsync(category.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(category.Id);

            // Assert
            result.Should().BeTrue();

            _partCategoryrepository.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _partCategoryrepository.Verify(x => x.DeleteAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _partCategoryrepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategory?)null);
            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeFalse();

            _partCategoryrepository.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once());
        }


        private static PartCategory GeneratePartCategory()
        {
            return new Faker<PartCategory>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Commerce.Department())
                .Generate();
        }


        private static CreatePartCategoryRequest GenerateCreatePartCategoryRequest()
        {
            return new Faker<CreatePartCategoryRequest>()
                .RuleFor(x => x.Name, f => f.Commerce.Department())
                .Generate();
        }

        private static UpdatePartCategoryRequest GenerateUpdateRequest()
        {
            return new Faker<UpdatePartCategoryRequest>()
                .RuleFor(x => x.Name, f => f.Commerce.Department())
                .Generate();
        }

    }
}
