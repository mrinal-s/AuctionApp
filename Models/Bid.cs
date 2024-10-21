namespace BidRestAPI.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class Bid
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bid amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Auction item ID is required.")]
        public Guid AuctionItemId { get; set; }

        public AuctionItem AuctionItem { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        public User User { get; set; }

        public DateTime BidTime { get; set; } = DateTime.UtcNow; // Default to current time
    }

}