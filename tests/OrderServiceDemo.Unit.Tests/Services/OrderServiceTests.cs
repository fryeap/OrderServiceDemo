using NSubstitute;
using NSubstitute.ReturnsExtensions;
using OrderServiceDemo.Models.Exceptions;
using OrderServiceDemo.Services.Components;
using OrderServiceDemo.Services.Infrastructure;
using OrderServiceDemo.Core;
using System.Threading.Tasks;
using Xunit;

namespace OrderServiceDemo.Unit.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineItemRepository _orderLineItemRepository;

        public OrderServiceTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _orderLineItemRepository = Substitute.For<IOrderLineItemRepository>();
        }

        [Fact]
        public async Task OrderService_WhenCreatingOrder_IfNoLineItems_ThrowsInvalidRequestException()
        {
            //Arrange
            var order = new Models.Order();
            var service = BuildService();

            //Act && Assert
            var result = await Assert.ThrowsAsync<InvalidRequestException>(() => service.CreateOrder(order));
        }

        [Fact]
        public async Task OrderService_WhenDeletingOrder_IfNoOrder_ThrowsInvalidRequestException()
        {
            //Arrange
            var service = BuildService();

            //Act && Assert
            var result = await Assert.ThrowsAsync<InvalidRequestException>(() => service.DeleteOrder(1));
        }

        [Fact]
        public async Task OrderService_WhenDeletingOrder_ReturnsDeletedOrder()
        {
            //Arrange
            var order = new Models.Order();
            order.OrderId = 1;
            _orderRepository.GetOrder(order.OrderId).Returns(order);
            _orderRepository.DeleteOrder(order).Returns(order);
            var service = BuildService();

            //Act
            var returnedOrder = await service.DeleteOrder(order.OrderId);

            //Assert
            //this test makes an assumption of data integrity. It would be better
            // to implement an equals method on the objects and do a deep check
            // It suffice for now to ensure the order id's are identical
            Assert.True(order.OrderId == returnedOrder.OrderId);
        }

        [Fact]
        public async Task OrderService_WhenCancellingOrder_UpdatesOrderStatus()
        {
            //Arrange
            var order = new Models.Order();
            order.OrderStatus = OrderStatus.Pending;
            order.OrderId = 1;
            _orderRepository.GetOrder(order.OrderId).Returns(order);
            _orderRepository.UpdateOrder(order).Returns(order);
            var service = BuildService();

            //Act
            await service.CancelOrder(order.OrderId);

            //Assert
            Assert.True(order.OrderStatus == OrderStatus.Cancelled);
        }

        [Fact]
        public async Task OrderService_WhenCancellingOrder_IfNoOrder_ThrowsInvalidRequestException()
        {
            //Arrange
            var orderId = 1;
            _orderRepository.GetOrder(orderId).ReturnsNull();
            var service = BuildService();

            //Act && Assert
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.CancelOrder(orderId));
        }

        [Fact]
        public async Task OrderService_WhenCancellingOrder_IfAlreadyCancelled_ThrowsInvalidRequestionException()
        {
            //Arrange
            var order = new Models.Order();
            order.OrderId = 1;
            order.OrderStatus = OrderStatus.Cancelled;
            _orderRepository.GetOrder(order.OrderId).Returns(order);
            var service = BuildService();

            //Act && Assert
            await Assert.ThrowsAsync<InvalidRequestException>(() => service.CancelOrder(order.OrderId));
        }

        private OrderService BuildService() => new OrderService(
            _orderRepository,
            _orderLineItemRepository);
    }
}
