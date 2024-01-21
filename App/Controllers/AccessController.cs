using App.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using App.Base.Constants;
using App.Web.Providers.Interface;
using System.Transactions;
using System.Text.Json;
using BC = BCrypt.Net.BCrypt;
using App.Base.Entities;
using App.Web.Data;
using Microsoft.EntityFrameworkCore;

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
                                e.Email == loginVM.Email);


                if (user == null)
                {
                    TempData["error"] = "Credential Not Matched !";
                    return View();
                }

                if(user.RecStatus != Status.Active)
                {
                    TempData["error"] = "Please contact to your Administrator";
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

                TempData["success"] = "Welcome " + user.FullName;

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
                u.IsActive = user.RecStatus == 'A' ? true : false;
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


        [HttpGet]
        public async Task<IActionResult> ActiveInactiveUser(int Id)
        {

            if(Id == null) throw new Exception("id");

            if (Id == _userProvider.GetUserId() || Id == 1)
            {
                TempData["error"] = "Currently logged in user cannot be inactive, please contact administrator";
                return RedirectToAction("AllUsers");
            }

            if (!_userProvider.IsAdmin()) throw new Exception("Unauthorized Access !");

            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Id);
            if (user == null) throw new Exception("User not Found !");

            if(user.RecStatus == 'A')
            {
                user.RecStatus = 'D';
            }
            else
            {
                user.RecStatus = 'A';
            }

            _db.Users.Update(user);
            _db.SaveChanges();
            TempData["success"] = "User you have selected has Active/Inactive !";
            return RedirectToAction(nameof(AllUsers));
        }



        [HttpGet]
        public async Task<IActionResult> PasswordReset(int UserId)
        {
            if (UserId == null) throw new Exception("User Id can't be a null !");
            if (UserId == 1) throw new Exception("Password can't be changed of SuperAdmin !");

            var user = _db.Users.Where(e => e.Id == UserId).FirstOrDefault();
            if (user == null)
            {
                TempData["error"] = "User Not Found !";
                return RedirectToAction(nameof(AllUsers));
            }

            var u = new PasswordResetVM()
            {
                UserId = user.Id,
                FullName = user.FullName,
                Password = ""
            };

            return View(u);
        }


        [HttpPost]
        public async Task<IActionResult> PasswordReset(PasswordResetVM vM)
        {
            if (ModelState.IsValid)
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    var user = await _db.Users.FirstOrDefaultAsync(e => e.Id == vM.UserId);
                    user.Password = BC.EnhancedHashPassword(vM.Password);

                    _db.Users.Update(user);
                    await _db.SaveChangesAsync();

                    scope.Complete();
                    TempData["success"] = "Password Changed Successfully !";
                    return RedirectToAction(nameof(AllUsers));
                }
                catch(Exception ex)
                {
                    scope.Dispose();
                    TempData["error"] = "Something Went Wrong ! " + ex;
                    return View(vM);
                }
            }
            return View(vM);
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
            return View(vm);
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

        //private readonly Random _random = new Random();

        //private string RandomPassword(int size, bool lowerCase = true)
        //{
        //    var builder = new StringBuilder(size);

        //    char offset = lowerCase ? 'a' : 'A';
        //    const int lettersOffset = 26;

        //    for (int i = 0; i < size; i++)
        //    {
        //        var @char = (char)_random.Next(offset, offset + lettersOffset);
        //        builder.Append(@char);
        //    }

        //    return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        //}
        #endregion PrivateMethods
    }
}
