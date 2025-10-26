using LibHub.UserService.Domain.Entities;

namespace LibHub.UserService.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
