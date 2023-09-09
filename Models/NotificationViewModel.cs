using System.ComponentModel.DataAnnotations;

namespace OnlineBiddingSystem.Models
{
    public class NotificationViewModel
    {
        [Key]
        public int Id { get; set; }

        public User? Sender { get; set; }
        public int? SenderId { get; set; }

        public User? Reciever { get; set; }
        public int? RecieverId { get; set; }


        public Bid? Bid { get; set; }
        public int BidId { get; set; }

        public DateTime NotificationTime { get; set; }

        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
