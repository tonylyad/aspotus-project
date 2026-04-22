using Aspotus.Catalog.Api.Controllers;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;
using AwesomeAssertions;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace Unit.Test.WebHost.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<IPartCategoryService> _serviceMock;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _serviceMock = new Mock<IPartCategoryService>();
            _controller = new CategoriesController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedResponse = GeneratePartCategoryResponse(id);

            _serviceMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(200);
            okResult!.Value.Should().BeEquivalentTo(expectedResponse);
            _serviceMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenMissing()
        {
            // Arrange
            var id = Guid.NewGuid();

            _serviceMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategoryResponse?)null);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            var notFoundResult = result as NotFoundResult;
            notFoundResult!.StatusCode.Should().Be(404);
            _serviceMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            var request = GeneratePartCategoryResponses(3);

            _serviceMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(request);

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(200);
            okResult!.Value.Should().BeEquivalentTo(request);
            _serviceMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenSuccessful()
        {
            // Arrange
            var request = GenerateCreatePartCategoryRequest();
            var createdId = Guid.NewGuid();
            var expectedResponse = new PartCategoryResponse { Id = createdId, Name = request.Name };

            _serviceMock
                .Setup(x => x.CreateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be("GetById");
            createdResult.RouteValues.Should().ContainKey("id");
            createdResult.RouteValues["id"].Should().Be(createdId);
            createdResult.Value.Should().BeEquivalentTo(expectedResponse);
            _serviceMock.Verify(x => x.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Update_ShouldReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GenerateUpdatePartCategoryRequest();
            var updatedResponse = new PartCategoryResponse { Id = id, Name = request.Name };

            _serviceMock
                .Setup(x => x.UpdateAsync(id, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedResponse);

            // Act
            var result = await _controller.Update(id, request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(200);
            okResult!.Value.Should().BeEquivalentTo(updatedResponse);
            _serviceMock.Verify(x => x.UpdateAsync(id, request, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Update_ShouldReturnNotFound()
        {
            // Arrange
            var request = GenerateUpdatePartCategoryRequest();

            _serviceMock
                .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategoryResponse?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            var notFoundResult = result as NotFoundResult;
            notFoundResult!.StatusCode.Should().Be(404);
            _serviceMock.Verify(x => x.UpdateAsync(It.IsAny<Guid>(), request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();

            _serviceMock
                .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            var notContentResult = result as NoContentResult;
            notContentResult!.StatusCode.Should().Be(204);
            _serviceMock.Verify(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            // Act
            var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            var notFoundResult = result as NotFoundResult;
            notFoundResult!.StatusCode.Should().Be(404);
        }

        private List<PartCategoryResponse> GeneratePartCategoryResponses(int count = 3)
        {
            return new Faker<PartCategoryResponse>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate(count);
        }

        private PartCategoryResponse GeneratePartCategoryResponse(Guid id)
        {
            return new Faker<PartCategoryResponse>()
                .RuleFor(b => b.Id, id)
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate();
        }

        private CreatePartCategoryRequest GenerateCreatePartCategoryRequest()
        {
            return new Faker<CreatePartCategoryRequest>()
                .RuleFor(r => r.Name, f => f.Company.CompanyName())
                .Generate();
        }

        private UpdatePartCategoryRequest GenerateUpdatePartCategoryRequest()
        {
            return new Faker<UpdatePartCategoryRequest>()
                .RuleFor(r => r.Name, f => $"{f.Company.CompanyName()} Updated")
                .Generate();
        }
    }
}
