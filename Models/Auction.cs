using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineBiddingSystem.Models
{
    public class Auction
    {
        [Key]
        public int Id { get; set; }

        public User? User { get; set; }
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Please enter auction title!")]
        public string Title { get; set; } = String.Empty;

        public Category? Category { get; set; }
        [Required(ErrorMessage="Please select category!")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Please enter auction price!")]
        public decimal Price { get; set; }

        public string ShortDesc { get; set; }

        [Required(ErrorMessage ="Please enter auction starting date!")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please enter auction ending date!")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Please enter auction description!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select auction cover picture!")]
        public string CoverPhoto { get; set; }

        public Location? Location { get; set; }
        [Required(ErrorMessage = "Please select location!")]
        public int LocationId { get; set; }

        public string? Status { get; set; }

        public int Bids { get; set; }

    }

    public class AuctionViewModel
    {
        public Auction? auctions { get; set; }

        public IEnumerable<SelectListItem>? categories { get; set; }

        public IEnumerable<SelectListItem>? locations { get; set; }
    }

    public class SearchAuctionViewModel
    {
        public List<Auction> auctions { get; set; }
    }
}
