using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBiddingSystem.Data;
using OnlineBiddingSystem.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace OnlineBiddingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpPost]
        public IActionResult CheckEmail(string email)
        {
            bool result = !_db.Users.ToList().Exists(p => p.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));
            return Json(result);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.Where(u => u.Email == model.Email).SingleOrDefault();

                if (user != null)
                {
                    if (user.Status == "Active")
                    {
                        bool isValid = (user.Password == model.Password);
                        if (isValid)
                        {
                            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Email) }, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                            HttpContext.Session.SetInt32("UserId", user.Id);
                            HttpContext.Session.SetString("Email", model.Email);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["errorPassword"] = "Invalid password..";
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["statusdeactive"] = "Your account status is locked, please try again later.";
                        return View(model);
                    }
                }
                else
                {
                    TempData["errorEmail"] = "Invalid email..";
                    return View(model);
                }
            }
            else
            {
                return View(model);
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


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Status = "Active";
                user.Country = "N/A";
                user.City = "N/A";
                user.Mobile = "";
                user.Address = "N/A";
                user.ResetPasswordCode = "";
                _db.Users.Add(user);
                _db.SaveChanges();
                TempData["successMessage"] = "Registerd Successfully, please login with your email!";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet("/account/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost("/account/forgot-password")]
        public IActionResult ForgotPassword(string EmailId)
        {
            string resetCode = Guid.NewGuid().ToString();

            var uriBuilder = new UriBuilder
            {
                Scheme = Request.Scheme,
                Host = Request.Host.Host,//A DNS-style domain name or IP address.
                Port = (int)Request.Host.Port,
                Path = $"/Account/ResetPassword/{resetCode}"
            };

            var link = uriBuilder.Uri.AbsoluteUri;

            var getuser = (from u in _db.Users where u.Email == EmailId select u).FirstOrDefault();
            if (getuser != null)
            {

                var subject = "Password Reset Request";
                var body = "Hi " + getuser.FirstName + ", <br/> You recently requested to reset your password for your account. Click the link below to reset it. " +
                    " <br/><br/><a href=" + link + ">Reset Password Link</a> <br/><br/>" +
                     "If you did not request a password reset, please ignore this email or reply to let us know.<br/><br/> Thank you";

                SendEmail(getuser.Email, body, subject);
                getuser.ResetPasswordCode = resetCode;
                _db.SaveChanges();
                TempData["successMessage"] = "Reset password link has been sent to your email id.";
            }
            else
            {
                TempData["errorMessage"] = "No Account found, try again.";
            }
            return View();
        }

        public void SendEmail(string emailAddress, string body, string subject)
        {
            var fromEmail = new MailAddress("musellers786@gmail.com", "Bidout");
            var toEmail = new MailAddress(emailAddress);
            var fromEmailPassword = "gvcrysukdhimmkyk";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = _db.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.Password = model.NewPassword;
                    user.ConfirmPassword = model.ConfirmPassword;
                    //make resetpasswordcode empty string now
                    user.ResetPasswordCode = "";
                    _db.SaveChanges();
                    return RedirectToAction(nameof(PasswordChanged));
                }
            }
            else
            {
                TempData["errorMessage"] = "Something invalid";
            }
            return View(model);
        }

        [HttpGet("/account/password-changed")]
        public IActionResult PasswordChanged()
        {
            TempData["successMessage"] = "Your password is changed successfully.";
            return View();
        }
    }
}
