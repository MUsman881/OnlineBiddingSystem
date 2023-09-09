using System.ComponentModel.DataAnnotations;

namespace OnlineBiddingSystem.Models
{
    public class Bid
    {
        [Key]
        public int BidId { get; set; }

        public Auction? Auction { get; set; } 
        public int AuctionId { get; set; }

        public User? User { get; set; }
        public int UserId { get; set; }

        public decimal BidAmount { get; set; }

        public DateTime Timestamp { get; set; }
    }

    public class BidHistoryViewModel
    {
        public List<Bid> bids { get; set; }
    }
}
