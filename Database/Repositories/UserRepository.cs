using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class UserRepository : IUserRepository
{
    // private readonly AppDbContext _context;
    //
    // public UserRepository(AppDbContext context)
    // {
    //     _context = context;
    // }

    // public async Task<User?> GetByEmailAsync(string email)
    // {
    //     return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    // }
    //
    // public async Task<User> CreateAsync(User user)
    // {
    //     _context.Users.Add(user);
    //     await _context.SaveChangesAsync();
    //     return user;
    // }
}