using Models.DTOs;

namespace Interfaces.IManager
{
   public interface IOrderManager
    {
        Task<Result> AddOrderAsync(OrderDto orderDto);
        Task<Result> DeleteOrderAsync(int id);
        Task<Result<List<OrderDto>>> GetAllOrdersAsync();
        Task<Result<OrderDto>> GetOrderById(int id);
    }
}
