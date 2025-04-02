using System.Diagnostics;
using AuthService.Models;
using Database.Models.DBModel;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;

namespace AuthService.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _client;

    public UserService(AppDbContext context, HttpClient client)
    {
        _context = context;
        _client = client;
    }

    public async Task<User> RegisterUserAsync(RegisterRequest req)
    {
        var span = new SpanAttributes();
        span.Add("db.system.name", "microsoft.sql_server");

        var activitySource = new ActivitySource("RegisterUserAsync");
        using var activity = activitySource.StartActivity("SampleOperation");

        activity?.SetTag("db.system.name", "microsoft.sql_server");

        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.Email == req.Email))
        {
            throw new Exception("User already exists.");
        }

        // Create user
        var user = new User { Name = req.Username, Email = req.Email, Password = req.Password };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _client.GetAsync("http://service_a:8080/sample");
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