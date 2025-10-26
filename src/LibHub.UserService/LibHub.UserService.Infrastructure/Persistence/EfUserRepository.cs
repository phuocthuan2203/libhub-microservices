using LibHub.UserService.Domain.Entities;
using LibHub.UserService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibHub.UserService.Infrastructure.Persistence;

public class EfUserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public EfUserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
