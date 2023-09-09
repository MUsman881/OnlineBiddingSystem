using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBiddingSystem.Data;
using OnlineBiddingSystem.Models;
using System.Net;

namespace OnlineBiddingSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                int userId = (int)HttpContext.Session.GetInt32("UserId");

                ViewBag.TodayBids = _db.Bids.Where(d => d.UserId == userId && d.Timestamp == DateTime.Now).Count();
                ViewBag.TotalBids = _db.Bids.Where(d=>d.UserId == userId).Count();
                ViewBag.TodayAuctions = _db.Auctions.Where(d => d.UserId == userId && d.StartDate == DateTime.Now).Count();
                ViewBag.TotalAuctions = _db.Auctions.Where(d=>d.UserId == userId).Count();

                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Auction()
        {
            return View();
        }

        public IActionResult Bids()
        {
            return View();
        }

        public IActionResult YourBids()
        {
            return View();
        }

        #region Auctions
        public IActionResult ShowAuctions()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            var auctions = _db.Auctions.Include(z => z.Category).Include(l => l.Location).Where(u => u.UserId == userId).ToList();
            return Json(new { data = auctions });
        }

        [HttpGet("user/create-auction/")]
        public IActionResult CreateAuction(int? id)
        {
            AuctionViewModel avm = new AuctionViewModel();
            
            avm.categories = _db.Categories.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            avm.locations = _db.Locations.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            avm.auctions = _db.Auctions.Find(id);

            if (id == null)
            {
                return View(avm);
            }
            if(avm == null)
            {
                return View();
            }
            return View(avm);
        }

        [HttpPost("user/create-auction/")]
        public IActionResult CreateAuction(AuctionViewModel auctionvm, IFormFile file)
        {
            int userid = (int)HttpContext.Session.GetInt32("UserId");
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (file != null)
            {
                string filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"assets\images\auctions");
                var extension = Path.GetExtension(file.FileName);


                using (var filestreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                {
                    file.CopyTo(filestreams);
                }
                auctionvm.auctions.CoverPhoto = @"\assets\images\auctions\" + filename + extension;
            }
            auctionvm.auctions.UserId = userid;

            if (auctionvm == null)
            {
                return RedirectToAction("Auction");
            }
            else
            {
                if (auctionvm.auctions.Id == 0)
                {
                    auctionvm.auctions.Status = "Approved";
                    _db.Auctions.Add(auctionvm.auctions);
                    _db.SaveChanges();
                    return RedirectToAction("Auction");
                }
                else

                {
                    auctionvm.auctions.Status = "Approved";
                    _db.Auctions.Update(auctionvm.auctions);
                    _db.SaveChanges();
                    return RedirectToAction("Auction");
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteAuction(int id)
        {
            var auctionFromDb = _db.Auctions.FirstOrDefault(u => u.Id == id);
            if (auctionFromDb == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                return Json(new { success = false, message = "Auction not found!" });
            }
            else
            {
                _db.Auctions.Remove(auctionFromDb);
                _db.SaveChanges();
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, message = "Auction deleted successfully." });
            }
        }
        #endregion

        #region User
        public IActionResult Profile()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            var user = _db.Users.Where(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            User profile = new User();
            profile = _db.Users.FirstOrDefault(u => u.Id == userId);
            return View(profile);
        }

        [HttpPost]
        public IActionResult ChangeProfile(EditProfileViewModel model)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Country = model.Country;
                user.City = model.City;
                user.Mobile = model.Mobile;

                _db.Users.Update(user);
                _db.SaveChanges();
                return Json(new { success = true, message = "Profile updated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "User not found!" });
            }
        }

        [HttpPost]
        public IActionResult ChangePassword(EditProfileViewModel model)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Password = model.Password;
                user.ConfirmPassword = model.ConfirmPassword;

                _db.Users.Update(user);
                _db.SaveChanges();
                return Json(new { success = true, message = "Password changed successfully." });
            }
            else
            {
                return Json(new { success = false, message = "User not found!" });
            }
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { success = false, message = "User not found!" });
            }
            else
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var storedCookies = Request.Cookies.Keys;
                foreach (var cookies in storedCookies)
                {
                    Response.Cookies.Delete(cookies);
                }
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, message = "Account deleted successfully." });
            }
        }
        #endregion
    }
}
