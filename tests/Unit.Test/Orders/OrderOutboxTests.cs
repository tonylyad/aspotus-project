using Aspotus.Orders.Api.Data.Entities;
using Aspotus.Orders.Api.Clients;
using Aspotus.Orders.Api.Data.Repositories.Interfaces;
using Aspotus.Orders.Api.Models.Requests;
using Aspotus.Orders.Api.Services.Implementations;
using Aspotus.Shared.IntegrationEvents;
using Aspotus.Shared.Messaging;
using AwesomeAssertions;
using Moq;
using System.Text.Json;

namespace Unit.Test.Orders;

public sealed class OrderOutboxTests
{
    [Fact]
    public async Task CreatePartOrder_ShouldPersistOrderAndOutboxMessageTogether()
    {
        // Arrange
        var repository = new Mock<IOrderRepository>();
        Order? savedOrder = null;
        OutboxMessage? savedMessage = null;

        repository
            .Setup(x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<OutboxMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<Order, OutboxMessage, CancellationToken>((order, message, _) =>
            {
                savedOrder = order;
                savedMessage = message;
            })
            .Returns(Task.CompletedTask);

        var catalogClient = new Mock<ICatalogInventoryClient>();
        catalogClient
            .Setup(x => x.ReserveAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid?>(),
                It.IsAny<IReadOnlyCollection<CatalogReservationItemRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid orderId, Guid? _, IReadOnlyCollection<CatalogReservationItemRequest> items, CancellationToken _) =>
                new CatalogReservationResponse
                {
                    OrderId = orderId,
                    Items = items.Select(item => new CatalogReservationItemResponse
                    {
                        ProductType = "Part",
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = 500,
                        Name = "Test part",
                        Article = "TEST-001"
                    }).ToList()
                });

        var service = new OrderService(repository.Object, catalogClient.Object);
        var request = new CreatePartOrderRequest
        {
            CustomerName = " Test Customer ",
            CustomerEmail = " customer@example.com ",
            CustomerPhone = " +79990000000 ",
            DeliveryAddress = " Test address ",
            Items =
            [
                new CreatePartOrderItemRequest
                {
                    PartId = Guid.NewGuid(),
                    PartName = " Test part ",
                    PartArticle = " TEST-001 ",
                    UnitPrice = 500,
                    Quantity = 2
                }
            ]
        };

        // Act
        var response = await service.CreatePartOrderAsync(
            request,
            Guid.NewGuid().ToString(),
            "user@example.com",
            "Test User");

        // Assert
        savedOrder.Should().NotBeNull();
        savedMessage.Should().NotBeNull();
        savedMessage!.Type.Should().Be(RabbitMqTopology.OrderCreatedRoutingKey);

        var integrationEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(savedMessage.Payload);
        integrationEvent.Should().NotBeNull();
        integrationEvent!.EventId.Should().Be(savedMessage.Id);
        integrationEvent.OrderId.Should().Be(response.Id);
        integrationEvent.TotalAmount.Should().Be(1000);
        integrationEvent.CustomerEmail.Should().Be("customer@example.com");

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<OutboxMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
