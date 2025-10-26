using LibHub.UserService.Domain.Interfaces;

namespace LibHub.UserService.Domain.Entities;

public class User
{
    public int UserId { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; }
    public string Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private User() { }

    public User(string username, string email, string hashedPassword, string role = "User")
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
        
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be null or empty", nameof(hashedPassword));

        Username = username;
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public bool VerifyPassword(string providedPassword, IPasswordHasher hasher)
    {
        return hasher.Verify(this.HashedPassword, providedPassword);
    }

    public void UpdateProfile(string username, string email)
    {
        if (!string.IsNullOrWhiteSpace(username))
            Username = username;
        
        if (!string.IsNullOrWhiteSpace(email))
            Email = email;
        
        UpdatedAt = DateTime.UtcNow;
    }
}
