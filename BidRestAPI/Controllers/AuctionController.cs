using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BidRestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;
        private readonly IAuthService _authService;
        private readonly ILogger<AuctionController> _logger; 

        public AuctionController(IAuctionService auctionService, IAuthService authService, ILogger<AuctionController> logger)
        {
            _auctionService = auctionService;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("get-auctions")]
        [AllowAnonymous] // Allow unauthenticated access to this endpoint if needed
        public async Task<IActionResult> GetAuctions()
        {
            try
            {
                var auctions = await _auctionService.GetAuctionItemsAsync();
                return Ok(auctions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching auction items.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving auctions. Please try again.");
            }
        }

        [HttpPost("create-auction")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Returns the validation errors
                }
                if (dto == null)
                {
                    _logger.LogWarning("Create auction failed: No data provided.");
                    return BadRequest("Auction data must be provided.");
                }

                if (dto.UserId == Guid.Empty)
                {
                    _logger.LogWarning("Create auction failed: User ID is missing.");
                    return BadRequest("User ID must be provided.");
                }

                // Validate if the user exists
                if (!await _authService.UserExistsAsync(dto.UserId))
                {
                    _logger.LogWarning($"Create auction failed: User with ID {dto.UserId} not found.");
                    return BadRequest("User ID does not exist.");
                }

                var auction = await _auctionService.CreateAuctionItemAsync(dto, dto.UserId);
                _logger.LogInformation($"Auction '{dto.Title}' created successfully for user {dto.UserId}.");

                return CreatedAtAction(nameof(GetAuctions), new { id = auction.Id }, auction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating auction.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the auction. Please try again.");
            }
        }

        [HttpDelete("delete-auction/{id:guid}")]
        public async Task<IActionResult> DeleteAuction(Guid id)
        {
            try
            {
                var result = await _auctionService.DeleteAuctionItemAsync(id);
                if (!result)
                {
                    _logger.LogWarning($"Auction with ID {id} not found.");
                    return NotFound(new { message = "Auction not found." });
                }

                return Ok(new { message = "Auction deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while deleting the auction. Please try again.");
            }
        }
    }
}
