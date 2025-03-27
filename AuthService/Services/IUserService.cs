using AuthService.Models;
using Database.Models;
using Database.Models.DBModel;

namespace AuthService.Services;

public interface IUserService
{
    Task<User> RegisterUserAsync(RegisterRequest request);
    Task<bool> Login(LoginRequest request);
    Task<bool> Logout(LogoutReq request);
}