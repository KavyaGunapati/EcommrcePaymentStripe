using Models.DTOs;

namespace Interfaces.IManager
{
    public interface IAuthManager
    {
        Task<Result> Register(Register register);
        Task<Result<AuthResponse>> LoginAsync(Login login);
        Task<Result> AssignRole(string userId,string role);
        Task<Result<AuthResponse>> Refreshtoken(RefreshRequest request);
    }
}
