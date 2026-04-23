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
    public class PartsControllerTests
    {
        private readonly Mock<IPartService> _serviceMock;
        private readonly PartsController _controller;

        public PartsControllerTests()
        {
            _serviceMock = new Mock<IPartService>();
            _controller = new PartsController(_serviceMock.Object);
        }


        [Fact]
        public async Task GetById_ShouldReturnOk_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedResponse = GeneratePartResponse(id);

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
                .ReturnsAsync((PartResponse?)null);

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
            var request = GeneratePartResponses(3);

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
            var request = GenerateCreatePartRequest();
            var createdId = Guid.NewGuid();
            var expectedResponse = new PartResponse { Id = createdId, Name = request.Name };

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
            var request = GenerateUpdatePartRequest();
            var updatedResponse = new PartResponse { Id = id, Name = request.Name };

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
            var request = GenerateUpdatePartRequest();

            _serviceMock
                .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartResponse?)null);

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

        private List<PartResponse> GeneratePartResponses(int count = 3)
        {
            return new Faker<PartResponse>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate(count);
        }

        private PartResponse GeneratePartResponse(Guid id)
        {
            return new Faker<PartResponse>()
                .RuleFor(b => b.Id, id)
                .RuleFor(b => b.Name, f => f.Company.CompanyName())
                .Generate();
        }

        private CreatePartRequest GenerateCreatePartRequest()
        {
            return new Faker<CreatePartRequest>()
                .RuleFor(r => r.Name, f => f.Company.CompanyName())
                .Generate();
        }

        private UpdatePartRequest GenerateUpdatePartRequest()
        {
            return new Faker<UpdatePartRequest>()
                .RuleFor(r => r.Name, f => $"{f.Company.CompanyName()} Updated")
                .Generate();
        }
    }
}