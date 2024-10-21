namespace BidRestAPI.Model
{
    using System.ComponentModel.DataAnnotations;

    public class UserRegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        public bool IsRecieveOutbidEmails { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
