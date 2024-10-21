namespace BidRestAPI.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class Bid
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Bid amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Auction item ID is required.")]
        public Guid AuctionItemId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current time
    }

}