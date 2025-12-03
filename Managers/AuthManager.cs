using DataAccess.Context;
using DataAccess.Entities;
using Interfaces.IManager;
using Managers.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;

namespace Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;
        public AuthManager(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;

        }
        public async Task<Result> AssignRole(string userId, string role)
        {
            try
            {
                var userExist = await _userManager.FindByIdAsync(userId);
                if (userExist == null) return new Result { Success = false, Message = "User Not found" };
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                var assign = await _userManager.AddToRoleAsync(userExist, role);
                return assign.Succeeded ? new Result { Success = true, Message = "Role Assigned" } : new Result { Success = false, Message = string.Join(", ", assign.Errors.Select(e => e.Description)) };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = ex.Message };
            }
        }

        public async Task<Result<AuthResponse>> LoginAsync(Login login)
        {
            var userExist = await _userManager.FindByEmailAsync(login.Email);
            if (userExist == null) { return new Result<AuthResponse> { Success = false, Message = "user Not Found" }; }
            var loggedin = await _userManager.CheckPasswordAsync(userExist, login.Password);
            if (loggedin == false) return new Result<AuthResponse> { Success = false, Message = "Unauthorized User" };
            var roles = await _userManager.GetRolesAsync(userExist);
            var accessToken = _tokenService.GenerateToken(userExist, roles);
            var refreshtk = _tokenService.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                Token = refreshtk,
                Expires = DateTime.Now.AddDays(7),
                UserId = userExist.Id,
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return new Result<AuthResponse> { Success = true, Message = "User login successfully", Data = new AuthResponse { AccessToken = accessToken, RefreshToken = refreshtk } };
        }

        public async Task<Result<AuthResponse>> Refreshtoken(RefreshRequest request)
        {
            var refreshToken = await _context.RefreshTokens.Include(u => u.User).FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.Expires <= DateTime.Now)
            {
                return new Result<AuthResponse> { Success = false, Message = "Token Expired" };
            }
            var user = refreshToken.User;
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateToken(user, roles);
            var refreshtk = _tokenService.GenerateRefreshToken();
            refreshToken.IsRevoked = true;
            var refToken = new RefreshToken
            {
                Token = refreshtk,
                Expires = DateTime.Now.AddDays(7),
                UserId = user.Id,
            };
           await _context.RefreshTokens.AddAsync(refToken);
            await _context.SaveChangesAsync();
            return new Result<AuthResponse> { Success = true, Message = "", Data = new AuthResponse { AccessToken = accessToken, RefreshToken = refreshtk } };
        }

        public async Task<Result> Register(Register register)
        {
            var userExist = await _userManager.FindByEmailAsync(register.Email);
            if (userExist != null) { return new Result { Success = false, Message = "user Already  Registered" }; }
            var newUser = new ApplicationUser
            {
                FullName = register.FullName,
                Email = register.Email,
                UserName = register.Email,
            };
            var result = await _userManager.CreateAsync(newUser, register.Password);
            if (!string.IsNullOrEmpty(register.Role))
            {
                if (!await _roleManager.RoleExistsAsync(register.Role))
                    await _roleManager.CreateAsync(new IdentityRole(register.Role));
                await _userManager.AddToRoleAsync(newUser, register.Role);
            }
            return result.Succeeded ? new Result { Success = true, Message = "Registered Successfully" } : new Result { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
        }
    }
}
