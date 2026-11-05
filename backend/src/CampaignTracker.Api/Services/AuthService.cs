using CampaignTracker.Api.Auth;
using CampaignTracker.Api.Data;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampaignTracker.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(AppDbContext db, TokenService tokenService, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    /// <summary>Creates a new user. Returns null if the email is already taken.</summary>
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var email = NormalizeEmail(request.Email);

        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            return null;
        }

        var user = new User
        {
            Email = email,
            DisplayName = request.DisplayName.Trim()
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreateResponse(user);
    }

    /// <summary>Returns a token for valid credentials, otherwise null.</summary>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var email = NormalizeEmail(request.Email);
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _db.SaveChangesAsync();
        }

        return CreateResponse(user);
    }

    public async Task<UserDto?> GetUserAsync(int userId)
    {
        return await _db.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDto(u.Id, u.Email, u.DisplayName))
            .SingleOrDefaultAsync();
    }

    private AuthResponse CreateResponse(User user)
    {
        var (token, expiresAt) = _tokenService.CreateToken(user);
        return new AuthResponse(token, expiresAt, new UserDto(user.Id, user.Email, user.DisplayName));
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
