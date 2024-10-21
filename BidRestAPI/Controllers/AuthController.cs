using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BidRestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Returns the validation errors
                }
                if (dto == null)
                {
                    _logger.LogWarning("Register attempt failed: No data provided.");
                    return BadRequest(new { message = "Registration data must be provided." });
                }

                var result = await _authService.RegisterAsync(dto);
                _logger.LogInformation($"User registered successfully with email {dto.Email}.");
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred during registration for email {dto?.Email ?? "unknown"}.");
                return StatusCode(500, new { message = "An error occurred while registering the user. Please try again." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Login attempt failed: No data provided.");
                    return BadRequest(new { message = "Login data must be provided." });
                }

                var token = await _authService.LoginAsync(dto);

                // Validate if token is returned
                if (string.IsNullOrWhiteSpace(token) || token == "Invalid login attempt")
                {
                    _logger.LogWarning($"Login attempt failed for email {dto.Email}: Token was not returned.");
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                _logger.LogInformation($"User logged in successfully with email {dto.Email}.");
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred during login for email {dto?.Email ?? "unknown"}.");
                return StatusCode(500, new { message = "An error occurred while logging in. Please try again." });
            }
        }

    }
}
