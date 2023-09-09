using System.ComponentModel.DataAnnotations;

namespace OnlineBiddingSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage="Enter First Name")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Last Name")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter your Emai")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Passwords do not match!")]
        public string ConfirmPassword { get; set; }

        public string? Status { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? Address { get; set; }

        public string? Mobile { get; set; }

        public string? ResetPasswordCode { get; set; }
    }
}
