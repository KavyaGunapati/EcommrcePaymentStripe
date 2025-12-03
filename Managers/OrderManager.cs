using AutoMapper;
using DataAccess.Entities;
using Interfaces.IManager;
using Interfaces.IRepository;
using Models.DTOs;
using Serilog;

namespace Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IGenericRepository<Order> _repository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        public OrderManager(IMapper mapper,IGenericRepository<Order> repository,IGenericRepository<Product> productRepository)
        {
            _repository=repository;
            _mapper=mapper;
            _productRepository=productRepository;
        }
        public async Task<Result> AddOrderAsync(OrderDto orderDto)
        {
            Log.Information("Add product for user {userId}",orderDto.Id);
            var order = new Order
            {
                Status = orderDto.Status,
            };
            await _repository.AddAsync(order);
            foreach (var item in orderDto.Items)
            {
                var product=await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    order.TotalAmount+= product.Price * item.Quantity;
                }
                var newItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    OrderId=order.Id,
                };
                order.OrderItems.Add(newItem);
                await _repository.Update(order);
            }
            await _repository.AddAsync(order);
            Log.Information("Order Added : {@Order}", order);
            return new Result { Success= true , Message="Order Added Successfully"};
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
