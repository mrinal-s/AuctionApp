using BidRestAPI.Data;
using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;  // Add logging
using Models;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly ILogger<AuthService> _logger;  // Inject a logger

    public AuthService(AppDbContext context, ITokenService tokenService, ILogger<AuthService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = new PasswordHasher<User>();
        _logger = logger;
    }

    
    public async Task<LoggedInUserDetail> LoginAsync(UserLoginDto loginDto)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning($"Login failed. No user found with email {loginDto.Email}.");
                return new LoggedInUserDetail { Message = "Invalid login attempt" };

            }

            // Verify the password hash
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result != PasswordVerificationResult.Success)
            {
                _logger.LogWarning($"Login failed for user {loginDto.Email}. Incorrect password.");
                return new LoggedInUserDetail { Message="Invalid login attempt" };
            }

            _logger.LogInformation($"User {loginDto.Email} logged in successfully.");
            return _tokenService.CreateToken(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during login.");
            throw new Exception("An unexpected error occurred. Please try again.");
        }
    }

    public async Task<string> RegisterAsync(UserRegisterDto registerDto)
    {
        try
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                _logger.LogWarning($"Registration failed. Email {registerDto.Email} is already registered.");
                return "Email is already registered";
            }

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                IsRecieveOutbidEmails = registerDto.IsRecieveOutbidEmails,
                UserName = !string.IsNullOrEmpty(registerDto.LastName) ? registerDto.FirstName : (registerDto.FirstName + " " + registerDto.LastName),
                Email = registerDto.Email,
                PasswordHash = _passwordHasher.HashPassword(null, registerDto.Password) // Hash the password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User {user.Email} registered successfully.");
            return "User registered successfully";
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error occurred during registration.");
            throw new Exception("A database error occurred. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during registration.");
            throw new Exception("An unexpected error occurred. Please try again.");
        }
    }
    public async Task<bool> UserExistsAsync(Guid userId)
    {
        try
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking if user with ID {userId} exists.");
            throw new Exception("An error occurred while checking user existence.");
        }
    }
}
