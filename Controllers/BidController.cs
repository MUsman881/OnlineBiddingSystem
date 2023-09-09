using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using OnlineBiddingSystem.Data;
using OnlineBiddingSystem.Models;
using Stripe.Checkout;
using System.Net;

namespace OnlineBiddingSystem.Controllers
{
    public class BidController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BidController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult PlaceBid(int auctionId, decimal bidAmount)
        {
            if (User.Identity.IsAuthenticated)
            {
                Bid bid = new Bid();
                Auction model = _db.Auctions.FirstOrDefault(x => x.Id == auctionId);

                if(bidAmount >= model.Price)
                {
                    bid.AuctionId = auctionId;
                    bid.BidAmount = bidAmount;
                    bid.UserId = (int)HttpContext.Session.GetInt32("UserId");
                    bid.Timestamp = DateTime.Now;

                    if (model != null)
                    {
                        model.Bids += 1;
                        _db.Auctions.Update(model);
                        _db.SaveChanges();
                    }

                    var bidResult = _db.Bids.Add(bid);
                    _db.SaveChanges();
                    if (bidResult != null)
                    {
                        return Json(new { Success = true, Message = "Congratulations, Your Bid has been placed!" });
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Unable to add bid, try again!" });
                    }
                }
                else
                {
                    return Json(new { Success = false, Message = "Bidding amount is less than actual amount." });
                }

            }
            else
            {
                return Json(new { Success = false, Message = "Please login first to place bid on this auction!" });
            }
        }

        [HttpPost]
        public IActionResult Deletebid(int id, int aucId)
        {
            var bid = _db.Bids.FirstOrDefault(u => u.BidId == id);
            Auction model = _db.Auctions.FirstOrDefault(a => a.Id == aucId);

            if (bid == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { success = false, message = "Auction Bid not found!" });
            }
            else
            {
                if (model != null)
                {
                    model.Bids -= 1;
                    _db.Auctions.Update(model);
                    _db.SaveChanges();
                }

                _db.Bids.Remove(bid);
                _db.SaveChanges();
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, message = "Your bid on this auction has been removed successfully." });
            }
        }

        public IActionResult BuyAuction(int id)
        {
            var auctionBid = _db.Bids.Where(b => b.BidId == id).Include(a => a.Auction).Include(u => u.User).FirstOrDefault();

            var domain = "https://localhost:7203/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"User/Dashboard",
                CancelUrl = domain + "User/Dashboard",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = auctionBid.User.Email,
            };

            var sessionList = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(auctionBid.BidAmount),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = auctionBid.Auction.Title
                    }
                },
                Quantity = 1
            };

            options.LineItems.Add(sessionList);

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult ShowAllBids()
        {
            var bids = _db.Bids.Include(a => a.Auction).Include(u=>u.Auction.User).Include(u => u.User).ToList();
            return Json(new { data = bids });
        }

        public IActionResult ShowBids()
        {
            var userid = (int)HttpContext.Session.GetInt32("UserId");
            var bids = _db.Bids.Include(a => a.Auction).Include(u=> u.User).Where(k=>k.Auction.UserId == userid).ToList();
            return Json(new { data = bids });
        }

        public IActionResult Getuserbids()
        {
            int userid = (int)HttpContext.Session.GetInt32("UserId");
            var userbids = _db.Bids.Include(a => a.Auction).Include(u => u.Auction.User).Where(k => k.UserId == userid).ToList();
            return Json(new { data = userbids });
        }
    }
}
