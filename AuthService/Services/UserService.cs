using AuthService.Models;
using Database.Models.DBModel;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;

namespace AuthService.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterUserAsync(RegisterRequest req)
    {
        var span = new SpanAttributes();
        span.Add("db.system.name", "microsoft.sql_server");
        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.Email == req.Email))
        {
            throw new Exception("User already exists.");
        }

        // Create user
        var user = new User { Name = req.Username, Email = req.Email, Password = req.Password };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> Login(LoginRequest req)
    {
        if (await _context.Users.AnyAsync(u => u.Email == req.Email && u.Password == req.Password))
        {
            return true;
        }

        return false;
    }

    public async Task<bool> Logout(LogoutReq req)
    {
        if (await _context.Users.AnyAsync(u => u.Email == req.Email))
        {
            return true;
        }

        return false;
    }
}