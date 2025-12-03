using DataAccess.Entities;
using Interfaces.IManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace EcommerceStripePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
       private readonly IProductManager _productManager;
        public ProductController(IProductManager productManager)
        {
            _productManager = productManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _productManager.GetAllProductAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _productManager.GetProductById(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto product)
        {
            try
            {
                var result = await _productManager.AddProductAsync(product);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDto product)
        {
            try
            {
                var result = await _productManager.UpdateProductAsync(product);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch(Exception ex) {
            
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productManager.DeleteProductAsync(id);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        [HttpGet("filter")]
        public async Task<IActionResult> FilteredProducts(int? minprice,int? maxprice,string? sortstring)
        {
            try
            {
                var result = await _productManager.FilterProducts(minprice, maxprice,sortstring);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
