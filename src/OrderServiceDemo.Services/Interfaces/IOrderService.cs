using OrderServiceDemo.Models;
using System.Threading.Tasks;

namespace OrderServiceDemo.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> GetOrder(int orderId);

        /// <summary>
        /// Creates the provided order. Throws <see cref="Models.Exceptions.InvalidRequestException"/>
        /// when incorrect data is supplied.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>The created order.</returns>
        Task<Order> CreateOrder(Order order);

        /// <summary>
        /// Cancels the order supplied. Throws <see cref="Models.Exceptions.InvalidRequestException"/>
        /// when an order id is supplied for an order that doesnt exist or
        /// is already <see cref="Core.OrderStatus.Cancelled"/>
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>The cancelled Order</returns>
        Task<Order> CancelOrder(int orderId);

        /// <summary>
        /// Deletes the order supplied. Throws <see cref="Models.Exceptions.InvalidRequestException"/>
        /// when an order id is upplied for an order that is already <see cref="Core.OrderStatus.Closed"/>
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>The deleted Order</returns>
        Task<Order> DeleteOrder(int orderId);
    }
}
