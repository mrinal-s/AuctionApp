using BidRestAPI.Data;
using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using BusinessLogic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;  
using Models;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;


namespace DataRepository
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;  // Inject a logger

        public UserService(AppDbContext context, ITokenService tokenService, ILogger<UserService> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
        }

        public async Task<string> UpdateUserAsync(UserUpdateDto updateDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(Guid.Parse(updateDto.Id));
                if (user == null)
                {
                    _logger.LogWarning($"Update failed. No user found with ID {updateDto.Id}.");
                    return "User not found";
                }

                // Update user fields
                user.FirstName = updateDto.FirstName ?? user.FirstName;
                user.LastName = updateDto.LastName ?? user.LastName;
                user.IsRecieveOutbidEmails = updateDto.IsRecieveOutbidEmails;
                user.UserName = !string.IsNullOrEmpty(updateDto.LastName) ? updateDto.FirstName : (updateDto.FirstName + " " + updateDto.LastName);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {user.Email} updated successfully.");
                return "User updated successfully";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred during user update.");
                throw new Exception("A database error occurred. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user update.");
                throw new Exception("An unexpected error occurred. Please try again.");
            }
        }

    }
}
