namespace BidRestAPI.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AuctionItem
    {
        [Key] // Optionally indicate that Id is the primary key
        public Guid Id { get; set; }  // UNIQUEIDENTIFIER corresponds to Guid

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Starting bid is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Starting bid must be greater than zero.")]
        public decimal StartingBid { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [FutureDate(ErrorMessage = "End date must be in the future.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserId { get; set; }  // Corresponds to the User's UNIQUEIDENTIFIER

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default to current date/time
    }

    // Custom Validation Attribute for future date
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime && dateTime <= DateTime.UtcNow)
            {
                return new ValidationResult(ErrorMessage ?? "Date must be in the future.");
            }
            return ValidationResult.Success;
        }
    }


}