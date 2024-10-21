using BidRestAPI.Model;

namespace BidRestAPI.Interfaces
{
    public interface IBidService
    {
        Task<Bid> PlaceBidAsync(Guid auctionItemId, decimal amount, Guid userId);
    }
}