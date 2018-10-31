using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderServiceDemo.Core;
using OrderServiceDemo.Core.v1;
using OrderServiceDemo.Core.v1.RequestModels;
using OrderServiceDemo.Models.Exceptions;
using OrderServiceDemo.Services.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OrderServiceDemo.Controllers
{
    public class OrderController : BaseServiceController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(
            IMapper mapper,
            IOrderService orderService)
        {
            _mapper = mapper;
            _orderService = orderService;
        }

        [HttpPost]
        [Route("v1/orders/create")]
        public async Task<Order> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var order = new Models.Order
            {
                OrderLineItems = request.OrderLineItems.Select(x => _mapper.Map<Models.OrderLineItem>(x)).ToList(),
                OrderStatus = OrderStatus.Pending,
                PurchasedOn = DateTime.Now,
                UserId = request.UserId
            };

            try
            {
                var createdOrder = await _orderService.CreateOrder(order);
                var response = _mapper.Map<Order>(createdOrder);
                return response;
            }
            catch(InvalidRequestException ex)
            {
                throw BuildExceptionResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /**
        * I went ahead and added this endpoint for convenience. I wanted to make
        * sure that I was properly creating/modifying orders via the otehr endpoints
        **/
        [HttpGet]
        [Route("v1/orders/{orderId:int}")]
        public async Task<Order> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrder(orderId);
            var response = _mapper.Map<Order>(order);
            return response;
        }

        [HttpPost]
        [Route("v1/orders/{orderId:int}/cancel")]
        public async Task<Order> CancelOrder(int orderId)
        {
            try
            {
                var order = await _orderService.CancelOrder(orderId);
                var response = _mapper.Map<Order>(order);
                return response;
            }
            catch(InvalidRequestException ex)
            {
                throw BuildExceptionResponse(HttpStatusCode.BadRequest, ex);
            }
            
        }

        [HttpDelete]
        [Route("v1/orders/{orderId:int}")]
        public async Task<Order> DeleteOrder(int orderId)
        {
            try
            {
                var order = await _orderService.DeleteOrder(orderId);
                var response = _mapper.Map<Order>(order);
                return response;
            }
            catch(InvalidRequestException ex)
            {
                throw BuildExceptionResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
