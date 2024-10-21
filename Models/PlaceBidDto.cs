namespace BidRestAPI.Model
{
    using System.ComponentModel.DataAnnotations;

    public class PlaceBidDto
    {
        [Required(ErrorMessage = "Bid amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }

}
