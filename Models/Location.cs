using System.ComponentModel.DataAnnotations;

namespace OnlineBiddingSystem.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Location Name is required.")]
        public string Name { get; set; }


    }
}
