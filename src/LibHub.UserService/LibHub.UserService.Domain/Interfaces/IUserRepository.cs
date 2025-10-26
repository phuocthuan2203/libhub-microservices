using LibHub.UserService.Domain.Entities;

namespace LibHub.UserService.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int userId);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
