using BidRestAPI.Model;

namespace BidRestAPI.Interfaces
{
    public interface IAuctionService
    {
        Task<List<AuctionItem>> GetAuctionItemsAsync();
        Task<AuctionItem> GetAuctionItemByIdAsync(Guid id);
        Task<AuctionItem> CreateAuctionItemAsync(CreateAuctionDto dto, Guid userId);
        Task<bool> DeleteAuctionItemAsync(Guid id);
    }


}
