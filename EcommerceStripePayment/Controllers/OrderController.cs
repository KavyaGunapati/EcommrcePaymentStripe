using DataAccess.Entities;
using Interfaces.IManager;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace EcommerceStripePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderManager _orderManager;
        public OrderController(IOrderManager orderManager)
        {
            _orderManager=orderManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderManager.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderManager.GetOrderById(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderAsync(OrderDto orderDto)
        {
            try
            {
                var result = await _orderManager.AddOrderAsync(orderDto);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderManager.DeleteOrderAsync(id);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
