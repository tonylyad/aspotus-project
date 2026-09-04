using Aspotus.Orders.Api.Clients;
using Aspotus.Orders.Api.Data.Entities;
using Aspotus.Orders.Api.Data.Repositories.Interfaces;
using Aspotus.Orders.Api.Enums;
using Aspotus.Orders.Api.Models.Requests;
using Aspotus.Orders.Api.Services.Implementations;
using AwesomeAssertions;
using Moq;

namespace Unit.Test.Orders;

public sealed class OrderStatusTests
{
    [Fact]
    public async Task CancelledOrder_ReleasesCatalogReservation()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Processing };
        var repository = new Mock<IOrderRepository>();
        repository.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
        repository.Setup(x => x.UpdateAsync(order, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var catalog = new Mock<ICatalogInventoryClient>();
        catalog.Setup(x => x.ReleaseAsync(order.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var service = new OrderService(repository.Object, catalog.Object);

        var result = await service.UpdateStatusAsync(order.Id, new UpdateOrderStatusRequest { Status = OrderStatus.Cancelled });

        result!.Status.Should().Be(nameof(OrderStatus.Cancelled));
        repository.Verify(x => x.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        catalog.Verify(x => x.ReleaseAsync(order.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
