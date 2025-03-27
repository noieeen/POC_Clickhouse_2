using AuthService.Models;
using Database.Data;
using Database.Models;
using Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterUserAsync(string name, string email)
    {
        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            throw new Exception("User already exists.");
        }

        // Create user
        var user = new User { Name = name, Email = email };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
    public async Task<bool> Login(string username, string password)
    {
        return await Task.FromResult(true);
    }

    public async Task<bool> Logout(string username)
    {
        return await Task.FromResult(true);
    }
}