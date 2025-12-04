using AutoMapper;
using DataAccess.Entities;
using Interfaces.IManager;
using Interfaces.IRepository;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Serilog;
using Serilog.Core;

namespace Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IGenericRepository<Order> _repository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IPaymentManager _paymentManager;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly ILogger<OrderManager> _logger;
        public OrderManager(IGenericRepository<OrderItem> orderItemRepository,
IPaymentManager paymentManager,
            ILogger<OrderManager> logger
,IMapper mapper,IGenericRepository<Order> repository,IGenericRepository<Product> productRepository)
        {
            _repository=repository;
            _mapper=mapper;
            _productRepository=productRepository;
            _paymentManager=paymentManager;
            _logger=logger;
            _orderItemRepository=orderItemRepository;

        }
        public async Task<Result> AddOrderAsync(OrderDto orderDto)
        {
            _logger.LogInformation("Starting AddOrderAsync for User {UserId}", orderDto.UserId);
            if (orderDto.Items == null || orderDto.Items.Count == 0)
            {

                _logger.LogWarning("Order has no items for User {UserId}", orderDto.UserId);
                return new Result { Success = false, Message = "Order must contain at least one item." };

            }
             var order = new Order
                {
                    UserId = orderDto.UserId,
                    Status = string.IsNullOrWhiteSpace(orderDto.Status) ? "Pending" :orderDto.Status,
                 CreatedAt = DateTime.UtcNow,
                 TotalAmount = 0m,
                 OrderItems = new List<OrderItem>()
             };

            await _repository.AddAsync(order);
            decimal total = 0m;
            foreach (var item in orderDto.Items)
            {
                var product=await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {

                    _logger.LogWarning("Product {ProductId} not found, aborting order {OrderId}", item.ProductId, order.Id);
                    return new Result { Success = false, Message = $"Product {item.ProductId} not found." };

                }

                if (item.Quantity <= 0)
                {
                    _logger.LogWarning("Invalid quantity {Quantity} for Product {ProductId}, aborting", item.Quantity, item.ProductId);
                    return new Result { Success = false, Message = "Quantity must be greater than zero." };
                }
                if (product.Stock < item.Quantity)
                {

                    _logger.LogWarning("Insufficient stock for Product {ProductId}. Requested {Req}, Available {Avail}", item.ProductId, item.Quantity, product.Stock);
                    return new Result { Success = false, Message = $"Insufficient stock for product {product.Name}." };
                }

                var unitPrice = product.Price;
                var lineTotal = unitPrice * item.Quantity;
                total += lineTotal;

                var newItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = unitPrice,
                    OrderId=order.Id,
                };
                await _orderItemRepository.AddAsync(newItem);
                product.Stock-=newItem.Quantity;
                await _productRepository.Update(product);
            }
            order.TotalAmount = total;
            await _repository.Update(order);

            _logger.LogInformation("Order {OrderId} created for User {UserId} with total {Total}", order.Id, order.UserId, order.TotalAmount);
            var paymentIntentResult = await _paymentManager.CreatePaymentIntentAsync(order.Id, order.TotalAmount);
            _logger.LogInformation("PaymentIntent created for Order {OrderId}: {IntentId}", order.Id, paymentIntentResult.PaymentIntentId);
            return new Result { Success= true , Message="Order Added Successfully",
                Data = new
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    PaymentIntentId = paymentIntentResult.PaymentIntentId,
                    ClientSecret = paymentIntentResult.ClientSecret
                }
            };

        }

        public async Task<Result> DeleteOrderAsync(int id)
        {
           var order=await _repository.GetByIdAsync(id);
            if (order == null) return new Result { Success = false, Message = "Order not found" };
            await _repository.Delete(order);
            return new Result { Success = true, Message = "Order deleted successfully" };
        }

        public async Task<Result<List<OrderDto>>> GetAllOrdersAsync()
        {
            var orders=await _repository.GetAllAsync();
            if (orders == null) return new Result<List<OrderDto>>{ Success = false, Message = "NO orders found" };
            return new Result<List<OrderDto>> { Success=true,Message="Orders Found",Data=_mapper.Map<List<OrderDto>>(orders) };
        }

        public async Task<Result<OrderDto>> GetOrderById(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return new Result<OrderDto> { Success = false, Message = "Order not found" };
            return new Result<OrderDto> {Success= true, Message ="Order found",Data=_mapper.Map<OrderDto>(order) };
        }
        
    }
}
