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
        private readonly UserManager<User> _userManager;
        private readonly ILogger<BidController> _logger;

        public BidController(IBidService bidService, UserManager<User> userManager, ILogger<BidController> logger)
        {
            _bidService = bidService;
            _userManager = userManager;
            _logger = logger;
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

                // Check if dto is null
                if (dto == null)
                {
                    _logger.LogWarning("PlaceBid attempt failed: No bid data provided.");
                    return BadRequest(new { message = "Bid data must be provided." });
                }

                // Retrieve the user ID from the claims
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Unauthorized bid attempt on auction {AuctionId}.", auctionItemId);
                    return Unauthorized(new { message = "User not authenticated." });
                }

                // Place bid
                var bid = await _bidService.PlaceBidAsync(auctionItemId, dto.Amount, userId);
                _logger.LogInformation("User {UserId} successfully placed a bid on auction {AuctionId}.", userId, auctionItemId);
                return Ok(bid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while placing a bid on auction {AuctionId} by user {UserId}.", auctionItemId, _userManager.GetUserId(User));
                return StatusCode(500, new { message = "An error occurred while placing the bid. Please try again." });
            }
        }
    }
}
