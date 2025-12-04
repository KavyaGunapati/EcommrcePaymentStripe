using DataAccess.Entities;
using Interfaces.IManager;
using Interfaces.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Stripe;

namespace EcommerceStripePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        
        private readonly IOrderManager _orderManager;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<Payment> _genericRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        public OrderController(IGenericRepository<Order> orderRepo,IOrderManager orderManager,IConfiguration configuration,IGenericRepository<Payment> genericRepository)
        {
            _orderManager=orderManager;
            _configuration=configuration;
            _genericRepository = genericRepository;
            _orderRepository = orderRepo;
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
        [HttpPost("payment-confirm")]
        public async Task<IActionResult> ConfirmPayment(string paymentId,string paymentMethodid)
        {

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var service = new PaymentIntentService();
            var confirmOptions = new PaymentIntentConfirmOptions
            {
                PaymentMethod = paymentMethodid,
            };
            try
            {
                var intent = await service.ConfirmAsync( paymentId,confirmOptions);
                var payment = (await _genericRepository.GetAllAsync()).FirstOrDefault(p => p.StripePaymentIntentId == paymentId);
                payment.Status = intent.Status;
                await _genericRepository.Update(payment);
                var order = (await _orderRepository.GetAllAsync()).FirstOrDefault(o => o.Id == payment.OrderId);
                order.Status = intent.Status != "Success" ? "Pending" : "Paid";
                return Ok(intent.Status);
            }
            catch (StripeException ex)
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
