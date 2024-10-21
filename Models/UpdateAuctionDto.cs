namespace BidRestAPI.Model
{
    public class UpdateAuctionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingBid { get; set; }
        public DateTime EndDate { get; set; }
    }

}
