using Database.Models;

namespace AuthService.Services;

public interface IUserService
{
    Task<User> RegisterUserAsync(string name, string email);
    Task<bool> Login(string email, string password);
    Task<bool> Logout(string email);
}