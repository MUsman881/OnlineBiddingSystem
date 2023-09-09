using Microsoft.AspNetCore.Mvc;
using OnlineBiddingSystem.Data;
using OnlineBiddingSystem.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OnlineBiddingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/about/")]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet("/how-it-works/")]
        public IActionResult Works()
        {
            return View();
        }

        [HttpGet("/all-auctions/")]
        public IActionResult Auctions()
        {
            return View();
        }

        [HttpGet("/auction-detail/")]
        public IActionResult AuctionDetails(int id)
        {
            ViewBag.auctionId = id;

            BidHistoryViewModel model = new BidHistoryViewModel();
            if (id > 0)
            {
                model.bids = _db.Bids.Include(u=>u.User).Where(a=>a.AuctionId == id).ToList();
            }

            if (model != null)
            {
                return View(model);
            }
            return View();
        }

        public IActionResult ViewAuctionDetails(int id)
        {
            var auctionDetails = _db.Auctions.Include(u => u.User).Where(a => a.Id == id).FirstOrDefault();
            return Json(auctionDetails);
        }

        public IActionResult GetAllCategories()
        {
            var categoryList = _db.Categories.ToList();
            return Json(new { data = categoryList });
        }

        public IActionResult GetAllAuctions()
        {
            var auctionList = _db.Auctions.Where(t => t.Status == "Approved" && t.EndDate >= DateTime.Now).Include(u=>u.User).Take(6).ToList();
            return Json(new { data = auctionList });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}