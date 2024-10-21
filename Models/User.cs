namespace BidRestAPI.Model
{

    using System;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }

        public bool IsRecieveOutbidEmails { get; set; }

        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid(); 

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password hash is required")]
        public string PasswordHash { get; set; }
    }




}