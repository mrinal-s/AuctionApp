using BidRestAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BidRestAPI.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace BidRestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BidController : ControllerBase
    {
        private readonly IBidService _bidService;
        private readonly IAuthService _authService;
        private readonly ILogger<BidController> _logger;

        public BidController(IBidService bidService, ILogger<BidController> logger, IAuthService authService)
        {
            _bidService = bidService;
            
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("{auctionItemId}")]
        public async Task<IActionResult> PlaceBid(Guid auctionItemId, [FromBody] PlaceBidDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Returns the validation errors
                }

                if (dto == null)
                {
                    _logger.LogWarning("PlaceBid attempt failed: No bid data provided.");
                    return BadRequest(new { message = "Bid data must be provided." });
                }

                if (dto.UserId == Guid.Empty)
                {
                    return BadRequest("User ID must be provided.");
                }

                // Validate if the user exists
                if (!await _authService.UserExistsAsync(dto.UserId))
                {
                    _logger.LogWarning($"PlaceBid attempt failed: User with ID {dto.UserId} not found.");
                    return BadRequest("User ID does not exist.");
                }
                // Place bid
                var bid = await _bidService.PlaceBidAsync(auctionItemId, dto.Amount, dto.UserId);
                return Ok(bid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while placing the bid. Please try again." });
            }
        }

    }
}
