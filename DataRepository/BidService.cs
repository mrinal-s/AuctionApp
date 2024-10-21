using BidRestAPI.Data;
using BidRestAPI.Interfaces;
using BidRestAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Add logging
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BidRestAPI.Services
{
    public class BidService : IBidService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BidService> _logger;  // Inject a logger

        public BidService(AppDbContext context, ILogger<BidService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Bid> PlaceBidAsync(Guid auctionItemId, decimal amount, Guid userId)
        {
            try
            {
                // Check if the auction item exists and is still active
                var auctionItem = await _context.AuctionItems.FirstOrDefaultAsync(a => a.Id == auctionItemId);
                if (auctionItem == null)
                {
                    _logger.LogWarning($"Bid attempt failed. Auction item with ID {auctionItemId} does not exist.");
                    throw new InvalidOperationException("The auction item does not exist.");
                }
                if (auctionItem.EndDate < DateTime.Now)
                {
                    _logger.LogWarning($"Bid attempt failed. Auction item with ID {auctionItemId} has ended.");
                    throw new InvalidOperationException("The auction has already ended.");
                }

                // Check the highest bid
                var highestBid = await _context.Bids
                    .Where(b => b.AuctionItemId == auctionItemId)
                    .OrderByDescending(b => b.Amount)
                    .FirstOrDefaultAsync();

                if (highestBid != null && amount <= highestBid.Amount)
                {
                    _logger.LogWarning($"Bid attempt failed. Bid amount {amount} is not higher than the current highest bid of {highestBid.Amount}.");
                    throw new InvalidOperationException("Your bid must be higher than the current highest bid.");
                }

                // Create a new bid
                var bid = new Bid
                {
                    Amount = amount,
                    AuctionItemId = auctionItemId,
                    UserId = userId.ToString(),
                    CreatedAt = DateTime.Now
                };

                _context.Bids.Add(bid);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Bid placed successfully for auction item {auctionItemId} by user {userId} with amount {amount}.");
                return bid;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error occurred while placing a bid for auction item {auctionItemId} by user {userId}.");
                throw new Exception("A database error occurred while placing the bid. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while placing a bid for auction item {auctionItemId} by user {userId}.");
                throw new Exception("An unexpected error occurred while placing the bid. Please try again.");
            }
        }
    }
}
