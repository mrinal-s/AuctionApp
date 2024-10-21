using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using BusinessLogic;
using DataRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;


namespace BidRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

       

        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Return validation errors
                }
                if (dto == null)
                {
                    _logger.LogWarning("Update attempt failed: No data provided.");
                    return BadRequest(new { message = "Update data must be provided." });
                }

                var result = await _userService.UpdateUserAsync(dto);

                if (result == "User not found")
                {
                    return NotFound(new { message = result });
                }

                _logger.LogInformation($"User {dto.Id} updated successfully.");
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating user {dto.Id}.");
                return StatusCode(500, new { message = "An error occurred while updating the user. Please try again." });
            }
        }
    }
}
