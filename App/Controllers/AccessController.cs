using App.Web.ViewModel;
using CTM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using App.Base.Constants;
using App.Model;
using App.Web.Providers.Interface;
using System.Transactions;
using System.Text.Json;
using BC = BCrypt.Net.BCrypt;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace App.Web.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDbContext _db;

        private readonly IUserProvider _userProvider;

        public AccessController(
            AppDbContext db,
            IUserProvider userProvider
            )
        {
            _db = db;
            _userProvider = userProvider;
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
                                e.Email == loginVM.Email && e.RecStatus == Status.Active);

                if (user == null)
                {
                    TempData["error"] = "Credential Not Matched !";
                    return View();
                }

                if (!VerifyPassword(loginVM.Password, user.Password))
                {
                    TempData["error"] = "Password Not Matched !";
                    return View();
                }

                SessionUserVM vm = new()
                {
                    Email = user.Email,
                    IsAdmin = user.IsAdmin,
                    UserId = user.Id,
                    UserName = user.FullName
                };

                var data = JsonSerializer.Serialize(vm);
                List<Claim> claims = new()
                {
                    new Claim("SessionUser", data),

                };

                AuthenticationProperties properties = new()
                {
                    AllowRefresh = true,
                    IsPersistent = true
                };

                ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);

                TempData["success"] = "User Successfully Logged In!";

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



        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            var users = await _db.Users.Where(e => e.Id != SuperAdmin.Admin).ToListAsync();
            var userVM = new List<UserVM>();

            foreach (var user in users)
            {
                var u = new UserVM();
                u.Id = user.Id;
                u.FullName = user.FullName;
                u.Email = user.Email;
                u.IsAdmin = user.IsAdmin == 'Y' ? "Yes" : "No";
                u.Status = user.RecStatus == 'A' ? "Active" : "InActive";
                userVM.Add(u);

            }
            return View(userVM);
        }



        [HttpGet]
        public IActionResult AddUser()
        {
            if (!_userProvider.IsAdmin())
            {
                TempData["error"] = "Permission Denied ! \\n You don't have permission to Access : " + Request.Path;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserVM user)
        {
            if (!_userProvider.IsAdmin())
            {
                TempData["error"] = "Permission Denied ! \\n You don't have permission to Access : " + Request.Path;
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {

                if (IsUniqueEmail(user))
                {
                    ModelState.AddModelError("Email", "Email is already Used !");
                }

                var admin = 'N';
                if (user.IsAdmin == "true")
                    admin = 'Y';


                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    var newUser = new User();
                    newUser.FullName = user.FullName;
                    newUser.Email = user.Email;
                    newUser.Password = BC.EnhancedHashPassword(user.Password);
                    newUser.IsNewPassword = 'Y';
                    newUser.RecBy = _userProvider.GetUsername();
                    newUser.IsAdmin = admin;

                    await _db.Users.AddAsync(newUser);
                    await _db.SaveChangesAsync();
                    scope.Complete();

                    TempData["success"] = "User Added Successfully !";
                    return RedirectToAction(nameof(AddUser));
                }
                catch (Exception)
                {
                    scope.Dispose();
                    throw new Exception("Something went wrong while User Add !");
                }

            }

            return View(user);
        }



        #region ChangePassword

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM vm)
        {
            if (ModelState.IsValid)
            {
                if (!NewAndConfirmPasswordSame(vm.NewPassword, vm.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Password & Confirm Password not matched !");
                    return View();
                }


                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    var user = await _db.Users.FirstOrDefaultAsync(e => e.Id == _userProvider.GetUserId());

                    if (user != null)
                    {

                        if (!VerifyPassword(vm.OldPassword, user.Password))
                        {
                            ModelState.AddModelError("OldPassword", "Recent Password not mached !");
                            return View();
                        }


                        user.Password = BC.EnhancedHashPassword(vm.NewPassword);
                        _db.Users.Update(user);
                        await _db.SaveChangesAsync();
                        scope.Complete();

                        TempData["success"] = "Password Change Succssfully !";
                        return RedirectToAction("Logout", "Home");
                    }
                    scope.Dispose();
                    return RedirectToAction(nameof(ChangePassword));

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("Something Went Wrong !\n" + ex.Message);
                }

            }
            return RedirectToAction(nameof(ChangePassword)); ;
        }

        #endregion ChangePassword


        #region PrivateMethods
        private bool IsUniqueEmail(UserVM vm)
        {
            var email = vm.Email;
            var data = _db.Users.FirstOrDefault(e => e.Email == email);
            if (data == null)
            {
                return true;
            }
            return false;
        }

        private bool VerifyPassword(string Password, string HashPassword)
        {
            if (BC.EnhancedVerify(Password, HashPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool NewAndConfirmPasswordSame(string NewPassword, string ConfirmPassword)
        {
            if (NewPassword == ConfirmPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion PrivateMethods
    }
}
