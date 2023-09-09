using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBiddingSystem.Data;
using OnlineBiddingSystem.Models;
using System.Net;
using System.Security.Claims;

namespace OnlineBiddingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("admin") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.TotalCategories = _db.Categories.Count();
                ViewBag.TodayBids = _db.Bids.Where(d => d.Timestamp == DateTime.Now).Count();
                ViewBag.TotalBids = _db.Bids.Count();
                ViewBag.TodayAuctions = _db.Auctions.Where(d => d.StartDate == DateTime.Now).Count();
                ViewBag.TotalAuctions = _db.Auctions.Count();
                ViewBag.TotalUsers = _db.Users.Count();
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Admin ad)
        {
            if (ModelState.IsValid)
            {
                var admin = _db.Admin.Where(u => u.email == ad.email).SingleOrDefault();

                if (admin != null)
                {
                    bool isValid = (admin.password == ad.password);
                    if (isValid)
                    {
                        HttpContext.Session.SetInt32("admin", admin.Id);
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        TempData["errorPassword"] = "Invalid password";
                        return View(ad);
                    }
                }
                else
                {
                    TempData["errorEmail"] = "Invalid email";
                    return View(ad);
                }
            }
            else
            {
                return View(ad);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var storedCookies = Request.Cookies.Keys;
            foreach (var cookies in storedCookies)
            {
                Response.Cookies.Delete(cookies);
            }
            return RedirectToAction("Login");
        }


        #region Category
        public IActionResult Category()
        {
            if (HttpContext.Session.GetInt32("admin") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        public IActionResult ShowCategory()
        {
            var categoryList = _db.Categories.ToList();
            return Json(new { data = categoryList });
        }

        [HttpPost]
        public IActionResult Upsert(Category category)
        {
            if (category == null)
            {
                return Json(new { success = false, message = "Please enter category name!" });
            }
            else
            {
                if (category.Id == 0)
                {
                    if (_db.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower()))
                    {
                        return Json(new { success = false, message = "Category already exists, try with another name!" });
                    }
                    else
                    {
                        _db.Categories.Add(category);
                        _db.SaveChanges();
                        return Json(new { success = true, message = "Category save successfully." });
                    }
                }
                else
                {
                    _db.Categories.Update(category);
                    _db.SaveChanges();
                    return Json(new { success = true, message = "Category updated successfully." });
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var catFromDb = _db.Categories.FirstOrDefault(u => u.Id == id);
            if (catFromDb == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                return Json(new { success = false, message = "Category not found!" });
            }
            else
            {
                _db.Categories.Remove(catFromDb);
                _db.SaveChanges();
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, message = "Category deleted successfully." });
            }
        }
        #endregion

        #region Location
        public IActionResult Location()
        {
            if (HttpContext.Session.GetInt32("admin") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        public IActionResult ShowLocation()
        {
            var locationList = _db.Locations.ToList();
            return Json(new { data = locationList });
        }

        [HttpPost]
        public IActionResult AddLocation(Location location)
        {
            if (location == null)
            {
                return Json(new { success = false, message = "Please enter location name!" });
            }
            else
            {
                if (location.Id == 0)
                {
                    if (_db.Locations.Any(c => c.Name.ToLower() == location.Name.ToLower()))
                    {
                        return Json(new { success = false, message = "Location already exists, try with another name!" });
                    }
                    else
                    {
                        _db.Locations.Add(location);
                        _db.SaveChanges();
                        return Json(new { success = true, message = "Location save successfully." });
                    }
                }
                else
                {
                    _db.Locations.Update(location);
                    _db.SaveChanges();
                    return Json(new { success = true, message = "Location updated successfully." });
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteLocation(int id)
        {
            var locFromDb = _db.Locations.FirstOrDefault(u => u.Id == id);
            if (locFromDb == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                return Json(new { success = false, message = "Location not found!" });
            }
            else
            {
                _db.Locations.Remove(locFromDb);
                _db.SaveChanges();
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, message = "Location deleted successfully." });
            }
        }
        #endregion

        #region Auction 
        [HttpGet("Admin/all-auctions")]
        public IActionResult Auction()
        {
            if (HttpContext.Session.GetInt32("admin") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Showallauctions()
        {
            var auctions = _db.Auctions.Include(z => z.Category).Include(l => l.Location).Include(u => u.User).ToList();
            return Json(new { data = auctions });
        }

        [HttpGet("Admin/Auction-detail")]
        public IActionResult Auctiondetails(int id)
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
            if (id == 0)
            {
                return NotFound();
            }
            avm.auctions = _db.Auctions.FirstOrDefault(c => c.Id == id);

            if (avm == null)
            {
                return NotFound();

            }
            return View(avm);
        }

        [HttpPost]
        public IActionResult ChangeStatus(int auctionId, string status)
        {
            Auction avm = _db.Auctions.Where(a => a.Id == auctionId).FirstOrDefault();

            avm.Status = status;
            _db.Auctions.Update(avm);
            _db.SaveChanges();

            return Json(new { success = true, message = "Status updated successfully." });
        }
        #endregion

        #region Users
        [HttpGet]
        public IActionResult Users()
        {
            if (HttpContext.Session.GetInt32("admin") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        [HttpGet("Admin/User-detail")]
        public IActionResult Userdetail(int id)
        {
            User model = new User();
            model = _db.Users.Where(u => u.Id == id).FirstOrDefault();

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangeUserStatus(int userId, string status)
        {
            User user = _db.Users.Where(a => a.Id == userId).FirstOrDefault();

            user.Status = status;
            _db.Users.Update(user);
            _db.SaveChanges();

            return Json(new { success = true, message ="The user " + user.FirstName + " is successfully " + status });
        }

        public IActionResult ShowAllUsers()
        {
            var userList = _db.Users.ToList();
            return Json(new { data = userList });
        }

        #endregion

        #region bids
        [HttpGet("Admin/all-bids")]
        public IActionResult Bid()
        {
            if (HttpContext.Session.GetInt32("admin") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}
