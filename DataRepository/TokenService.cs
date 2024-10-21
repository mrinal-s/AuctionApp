using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BidRestAPI.Model;
using BidRestAPI.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;  // Add logging
using System;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;  // Inject a logger

    public TokenService(IConfiguration config, ILogger<TokenService> logger)
    {
        _configuration = config;
        _logger = logger;

        try
        {
            var secretKey = config["Jwt:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                _logger.LogError("JWT secret key is missing in configuration.");
                throw new ArgumentNullException("Jwt:Secret", "JWT secret key is missing in the configuration.");
            }

            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)); // Fetch key from config
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing TokenService. JWT secret key could not be set.");
            throw new Exception("Error initializing TokenService. Please check the JWT configuration.");
        }
    }

    public string CreateToken(User user)
    {
        try
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation($"Token created successfully for user {user.Email}.");
            return tokenHandler.WriteToken(token);
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogError(ex, "Error creating the JWT token.");
            throw new Exception("An error occurred while generating the token.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during token creation.");
            throw new Exception("An unexpected error occurred. Please try again.");
        }
    }
}
