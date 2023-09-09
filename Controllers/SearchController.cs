using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineBiddingSystem.Data;
using OnlineBiddingSystem.Models;

namespace OnlineBiddingSystem.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ApplicationDbContext _db;

        public SearchController(ILogger<SearchController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("/search/search-auction")]
        public IActionResult SearchAuction(string? category, string q)
        {
            SearchAuctionViewModel model = new SearchAuctionViewModel();

            if (!string.IsNullOrEmpty(category))
            {
                model.auctions = _db.Auctions.AsQueryable().Where(x => x.Category.Name == category).Include(u => u.User).ToList();
            }
            if (!string.IsNullOrEmpty(q))
            {
                model.auctions = _db.Auctions.AsQueryable().Where(x => x.Title.ToLower().Contains(q.ToLower())).Include(u => u.User).ToList();
            }
            
            if (model != null)
            {
                return View(model);
            }
            return View();
        }
    }
}
