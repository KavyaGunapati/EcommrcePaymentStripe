using Interfaces.IManager;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace EcommerceStripePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(Register register)
        {
            try
            {
                var result = await _authManager.Register(register);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                var result = await _authManager.LoginAsync(login);
                return result.Success ? Ok(result) : Unauthorized(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(string userID, string role)
        {
            try
            {
                var result = await _authManager.AssignRole(userID, role);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest token)
        {
            try
            {
                var result = await _authManager.Refreshtoken(token);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
