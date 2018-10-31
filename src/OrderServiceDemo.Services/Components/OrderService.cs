using System.Linq;
using System.Threading.Tasks;
using OrderServiceDemo.Core;
using OrderServiceDemo.Models;
using OrderServiceDemo.Models.Exceptions;
using OrderServiceDemo.Services.Infrastructure;
using OrderServiceDemo.Services.Interfaces;

namespace OrderServiceDemo.Services.Components
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineItemRepository _orderLineItemRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderLineItemRepository orderLineItemRepository)
        {
            _orderRepository = orderRepository;
            _orderLineItemRepository = orderLineItemRepository;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            if (order.OrderLineItems?.Any() != true)
                throw new InvalidRequestException("To create an order you must supply at least 1 line item");

            var createdOrder = await _orderRepository.CreateOrder(order);

            foreach(var lineItem in order.OrderLineItems)
            {
                lineItem.OrderId = createdOrder.OrderId;
            }

            var lineItems = await Task.WhenAll(order.OrderLineItems.Select(x => _orderLineItemRepository.CreateOrderLineItem(x)));
            createdOrder.OrderLineItems = lineItems.ToList();
            return createdOrder;
        }

        public async Task<Order> GetOrder(int orderId)
        {
            var order = await _orderRepository.GetOrder(orderId);
            await BuildUpOrder(order);
            return order;
        }

        public async Task<Order> CancelOrder(int orderId)
        {
            //TODO: Add Unit tests for this service method.
            var order = await GetOrder(orderId);
            if (order == null) {
                throw new InvalidRequestException("There is no order with that ID to be cancelled.");
            } else if (order.OrderStatus == OrderStatus.Cancelled) {
                throw new InvalidRequestException("This order has already been cancelled.");
            }

            order.OrderStatus = OrderStatus.Cancelled;
            //could streamline this by not waiting and immediately returning the order
            // we send off to the repository service but that may be a bad idea givet
            // that something could go wrong with the update. There's a trade off
            // between speed/efficiency and trusting the response
            order = await _orderRepository.UpdateOrder(order);
            return order;
        }

        public async Task<Order> DeleteOrder(int orderId)
        {
            //TODO: Add Unit tests for this service method.
            var order = await GetOrder(orderId);
            if (order == null) {
                throw new InvalidRequestException("No order found to delete.");
            }
            //delete line items first - then delete order
            var lineItems = await _orderLineItemRepository.DeleteAllLineItemsInOrder(orderId);
            order = await _orderRepository.DeleteOrder(order);
            return order;
        }

        private async Task<Order> BuildUpOrder(Order order)
        {
            if (order == null)
                return order;

            var lineItems = await _orderLineItemRepository.GetOrderLineItems(order.OrderId);
            order.OrderLineItems = lineItems.ToList();
            return order;
        }
    }
}
