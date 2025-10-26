using LibHub.UserService.Application.DTOs;
using LibHub.UserService.Application.Interfaces;
using LibHub.UserService.Domain.Entities;
using LibHub.UserService.Domain.Interfaces;

namespace LibHub.UserService.Application.Services;

public class IdentityApplicationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public IdentityApplicationService(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher, 
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<UserDto> RegisterUserAsync(RegisterUserDto registerDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists.");

        var hashedPassword = _passwordHasher.Hash(registerDto.Password);

        var user = new User(registerDto.Username, registerDto.Email, hashedPassword, "User");

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<TokenDto> LoginUserAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.VerifyPassword(loginDto.Password, _passwordHasher))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new TokenDto { AccessToken = token };
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email
        };
    }
}
