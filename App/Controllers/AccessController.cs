using App.Web.ViewModel;
using CTM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using App.Base.Constants;
using App.Model;

namespace App.Web.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDbContext _db;

        public AccessController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!String.IsNullOrEmpty(returnUrl))
            {
                Response.Cookies.Append("returnUrl", returnUrl);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FirstOrDefaultAsync(e =>
                                e.Email == loginVM.Email
                                && e.Password == loginVM.Password
                                && e.RecStatus == Status.Active);

                if (user == null)
                {
                    TempData["error"] = "Credential Not Matched !";
                    return View();
                }

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim("Email", user.Email),
                    new Claim("Username", user.Username),
                    new Claim("UserID", Convert.ToString(user.Id))
                };

                AuthenticationProperties properties = new()
                {
                    AllowRefresh = true,
                    IsPersistent = true
                };

                ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("UserID", Convert.ToString(user.Id));


                var returnUrl = Request.Cookies["returnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    Response.Cookies.Delete("returnUrl");
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
