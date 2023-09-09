

using System.ComponentModel.DataAnnotations;

namespace OnlineBiddingSystem.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter your Email")]
        public string email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter your Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
