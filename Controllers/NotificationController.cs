using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBiddingSystem.Data;
using OnlineBiddingSystem.Models;

namespace OnlineBiddingSystem.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public NotificationController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("Notification/Notification-detail")]
        public IActionResult Notificationdetail(int id)
        {
            var recieverId = (int)HttpContext.Session.GetInt32("UserId");
            var notification = _db.Notifications.Where(n => n.Id == id).Include(b => b.Bid).Include(a => a.Bid.Auction).Include(u => u.Bid.Auction.User).Where(u => u.Reciever.Id == recieverId).FirstOrDefault();
            notification.Status = "Read";

            _db.Notifications.Update(notification);
            _db.SaveChanges();

            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        [HttpPost]
        public IActionResult NotifyUser(int bidid, int senderid, int recieverid)
        {
            var user = _db.Users.Where(u => u.Id == senderid).FirstOrDefault();
            NotificationViewModel model = new NotificationViewModel();

            if (bidid > 0)
            {
                model.BidId = bidid;
                model.SenderId = senderid;
                model.RecieverId = recieverid;
                model.NotificationTime = DateTime.Now;
                model.Status = "Unread";
                model.Message = "You've recieved a bidding notification from seller.";

                _db.Notifications.Add(model);
                _db.SaveChanges();

                return Json(new { Success = true, Message = "Bidding Notification has been sent to respective user." });
            }
            else
            {
                return Json(new { Success = false, Message = "Error, please try again later." });
            }
        }

        public IActionResult GetNotification()
        {
            var recieverId = (int)HttpContext.Session.GetInt32("UserId");
            var notification = _db.Notifications.Include(b => b.Bid).Include(u => u.Sender).Where(u => u.Reciever.Id == recieverId).ToList();
            return Json(new { data = notification });
        }
    }
}
