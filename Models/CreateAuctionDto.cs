namespace BidRestAPI.Model
{
    public class CreateAuctionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingBid { get; set; }
        public DateTime EndDate { get; set; }
        public Guid UserId { get; set; }  // Ensure this matches the UserId type in your AuctionItems table
    }

}
