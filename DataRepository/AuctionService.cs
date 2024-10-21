using BidRestAPI.Data;
using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
namespace BidRestAPI.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuctionService> _logger; 

        public AuctionService(AppDbContext context, ILogger<AuctionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AuctionItem>> GetAuctionItemsAsync()
        {
            try
            {
                return await _context.AuctionItems.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching auction items.");
                throw new Exception("An error occurred while retrieving auction items. Please try again.");
            }
        }

        public async Task<AuctionItem> GetAuctionItemByIdAsync(Guid id)
        {
            try
            {
                var auctionItem = await _context.AuctionItems.Include(a => a.UserId).FirstOrDefaultAsync(a => a.Id == id);
                if (auctionItem == null)
                {
                    _logger.LogWarning($"Auction item with ID {id} not found.");
                    throw new KeyNotFoundException("Auction item not found.");
                }
                return auctionItem;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching auction item with ID {id}.");
                throw new Exception("An error occurred while retrieving the auction item. Please try again.");
            }
        }

        public async Task<AuctionItem> CreateAuctionItemAsync(CreateAuctionDto dto, Guid userId)
        {
            try
            {
                var auctionItem = new AuctionItem
                {
                    Id = Guid.NewGuid(),  // Generate a new GUID
                    Title = dto.Title,
                    Description = dto.Description,
                    StartingBid = dto.StartingBid,
                    EndDate = dto.EndDate,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AuctionItems.Add(auctionItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Auction item '{dto.Title}' created successfully by user {userId}.");
                return auctionItem;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while creating auction item.");
                throw new Exception("An error occurred while creating the auction item. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating auction item.");
                throw new Exception("An unexpected error occurred. Please try again.");
            }
        }

        public async Task<bool> DeleteAuctionItemAsync(Guid id)
        {
            try
            {
                var auctionItem = await _context.AuctionItems.FirstOrDefaultAsync(a => a.Id == id);
                if (auctionItem == null)
                {
                    _logger.LogWarning($"Auction item with ID {id} not found.");
                    return false;
                }

                _context.AuctionItems.Remove(auctionItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Auction item with ID {id} deleted successfully.");
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error occurred while deleting auction item with ID {id}.");
                throw new Exception("An error occurred while deleting the auction item. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred while deleting auction item with ID {id}.");
                throw new Exception("An unexpected error occurred. Please try again.");
            }
        }
    }
}
