using System.ComponentModel.DataAnnotations;

namespace OnlineBiddingSystem.Models
{
    public class EditProfileViewModel
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string? ConfirmPassword { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; } 

        public string? Mobile { get; set; }
    }
}
