# UserService - Internal Architecture Design

## 1. Guiding Architectural Pattern: Clean Architecture

This document outlines the internal architecture for the `UserService`. The service is built following the principles of **Clean Architecture**.

The core principle is **The Dependency Rule**: all source code dependencies must point inwards, from outer layers to inner layers. The `Domain` layer at the core must have zero dependencies on any other layer. This ensures the business logic is independent of infrastructure concerns like databases or frameworks, making the application robust, testable, and maintainable.

The implementation will be structured into four separate C# projects, representing the four layers.

## 2. Architectural Diagram (Template)

The following diagram illustrates the standard four-layer structure and the direction of dependencies.

```plantuml
@startuml
!theme plain
skinparam componentStyle uml2

title Clean Architecture for a Single Microservice

package "Presentation Layer (Web API)" {
  [API Controllers]
  [Middleware]
  [Dependency Injection]
}

package "Application Layer (Use Cases)" {
  [Application Services]
  [DTOs]
  [Interfaces for Infrastructure]
  [Validation]
}

package "Domain Layer (Core Business Rules)" {
  [Entities (Aggregates)]
  [Repository Interfaces]
}

package "Infrastructure Layer (Implementations)" {
  [EF Core DbContext]
  [Repository Implementations]
  [External Service Clients]
}

' --- Dependency Arrows ---
Presentation_Layer --> Application_Layer
Application_Layer --> Domain_Layer
Infrastructure_Layer --> Application_Layer
Infrastructure_Layer --> Domain_Layer

note right of Infrastructure_Layer
  Implements interfaces
  defined in the inner layers.
end note

note bottom of Domain_Layer
  **The Dependency Rule:** All arrows point inwards.
  The core Domain Layer has zero external dependencies.
end note

@enduml
3. Layer-by-Layer Breakdown
3.1. Domain Layer (LibHub.UserService.Domain)

This is the core of the service, containing the business logic and objects for user identity.

Entity: User.cs

This is the Aggregate Root. It encapsulates the data and the rules that govern a user's identity.

code
C#
download
content_copy
expand_less
public class User
{
    public int UserId { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; }
    public string Role { get; private set; }

    // Constructor for creating a new user
    public User(string username, string email, string hashedPassword, string role)
    {
        // Validation logic (e.g., ensure email is valid, role is correct) can go here
        Username = username;
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
    }

    // Business logic method
    public bool VerifyPassword(string providedPassword, IPasswordHasher hasher)
    {
        return hasher.Verify(this.HashedPassword, providedPassword);
    }
}
Repository Interface: IUserRepository.cs

This defines the contract for how user data is persisted, without specifying the technology.

code
C#
download
content_copy
expand_less
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int userId);
    Task AddAsync(User user);
}
3.2. Application Layer (LibHub.UserService.Application)

This layer orchestrates the use cases for the UserService.

Data Transfer Objects (DTOs):

RegisterUserDto.cs: { string Username, string Email, string Password }

LoginDto.cs: { string Email, string Password }

UserDto.cs: { int UserId, string Username, string Email }

TokenDto.cs: { string AccessToken }

Infrastructure Interfaces: These abstract away external concerns, making the application logic testable.

IPasswordHasher.cs: interface { string Hash(string password); bool Verify(string hash, string password); }

IJwtTokenGenerator.cs: interface { string GenerateToken(User user); }

Application Service: IdentityApplicationService.cs
This service contains the core logic for the registration and login use cases.

code
C#
download
content_copy
expand_less
public class IdentityApplicationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public IdentityApplicationService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator) 
    { 
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<UserDto> RegisterUserAsync(RegisterUserDto registerDto)
    {
        // 1. Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null) throw new Exception("User with this email already exists.");

        // 2. Hash the password
        var hashedPassword = _passwordHasher.Hash(registerDto.Password);

        // 3. Create domain entity
        var user = new User(registerDto.Username, registerDto.Email, hashedPassword, "User");

        // 4. Persist using the repository
        await _userRepository.AddAsync(user);

        // 5. Map to DTO and return
        return new UserDto { /* mapping logic */ };
    }

    public async Task<TokenDto> LoginUserAsync(LoginDto loginDto)
    {
        // 1. Get user by email
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null) throw new Exception("Invalid credentials.");

        // 2. Verify password (delegated to the entity and hasher)
        if (!user.VerifyPassword(loginDto.Password, _passwordHasher))
        {
            throw new Exception("Invalid credentials.");
        }

        // 3. Generate JWT
        var token = _jwtTokenGenerator.GenerateToken(user);

        // 4. Return token DTO
        return new TokenDto { AccessToken = token };
    }
}
3.3. Infrastructure Layer (LibHub.UserService.Infrastructure)

This layer contains the concrete implementations of the interfaces defined in the inner layers.

Persistence: EfUserRepository.cs

Implements IUserRepository using Entity Framework Core.

Will contain a UserDbContext class.

Uses _context.Users.FirstOrDefaultAsync(...) and _context.Users.AddAsync(...) to perform data operations.

Security: PasswordHasher.cs

Implements IPasswordHasher using a library like BCrypt.Net.BCrypt.

Security: JwtTokenGenerator.cs

Implements IJwtTokenGenerator using the System.IdentityModel.Tokens.Jwt library.

Will read the secret key and issuer details from IConfiguration.

3.4. Presentation Layer (LibHub.UserService.Api)

This is the ASP.NET Core Web API project that acts as the entry point.

Controller: UsersController.cs

The controller is "thin." Its only job is to receive HTTP requests, call the application service, and return an HTTP response.

code
C#
download
content_copy
expand_less
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IdentityApplicationService _identityService;

    public UsersController(IdentityApplicationService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerDto)
    {
        var userDto = await _identityService.RegisterUserAsync(registerDto);
        return CreatedAtAction(nameof(GetUserById), new { id = userDto.UserId }, userDto);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var tokenDto = await _identityService.LoginUserAsync(loginDto);
        return Ok(tokenDto);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id) { /* Implementation using the service */ }
}

Dependency Injection (Program.cs):

This file wires everything together, for example:

services.AddDbContext<UserDbContext>(...)

services.AddScoped<IUserRepository, EfUserRepository>();

services.AddScoped<IdentityApplicationService>();

services.AddSingleton<IPasswordHasher, PasswordHasher>();

services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
