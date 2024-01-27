using App.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using App.Base.Constants;
using System.Transactions;
using System.Text.Json;
using BC = BCrypt.Net.BCrypt;
using App.Base.Entities;
using App.Base.Providers.Interfaces;
using App.Base.Repository.Interfaces;
using App.Base.ValueObject;
using App.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Web.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDbContext _db;

        private readonly ILoginUserProvider _userProvider;
        private readonly IUserRepository _userRepository;

        public AccessController(
            AppDbContext db,
            ILoginUserProvider userProvider,
            IUserRepository userRepository
        )
        {
            _db = db;
            _userProvider = userProvider;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                return RedirectToAction("Index", "Home");
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                Response.Cookies.Append("returnUrl", returnUrl);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _userRepository.GetFirstOrDefaultAsync(e => e.Email == loginVM.Email);


            if (!ValidateUserCredential(this, loginVM, user))
            {
                return View(loginVM);
            }

            SessionUser vm = new()
            {
                UserId = user.Id,
                UserName = user.FullName,
                Email = user?.Email,
                IsAdmin = user.IsAdmin
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

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), properties);

            TempData["success"] = "Welcome " + user.FullName;

            var returnUrl = Request.Cookies["returnUrl"];
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Home");
            Response.Cookies.Delete("returnUrl");
            return Redirect(returnUrl);
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
            if (_userProvider.IsAdmin()) return View();
            TempData["error"] = "Permission Denied ! \\n You don't have permission to Access : " + Request.Path;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserVM vm)
        {
            if (!_userProvider.IsAdmin())
            {
                TempData["error"] = "Permission Denied ! \\n You don't have permission to Access : " + Request.Path;
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid) return View(vm);

            if (!await IsUniqueEmail(vm.Email))
            {
                ModelState.AddModelError("Email", "Email is already Used !");
                return View(vm);
            }

            var admin = 'N';
            if (vm.IsAdmin == "true")
                admin = 'Y';


            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var newUser = new User();
                newUser.FullName = vm.FullName;
                newUser.Email = vm.Email;
                newUser.Password = BC.EnhancedHashPassword(vm.Password);
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

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> ActiveInactiveUser(int Id)
        {
            if (Id == null) throw new Exception("id");

            if (Id == _userProvider.GetUserId() || Id == 1)
            {
                TempData["error"] = "Currently logged in user cannot be inactive, please contact administrator";
                return RedirectToAction("AllUsers");
            }

            if (!_userProvider.IsAdmin()) throw new Exception("Unauthorized Access !");

            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Id);
            if (user == null) throw new Exception("User not Found !");

            if (user.RecStatus == 'A')
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
                catch (Exception ex)
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
            if (!ModelState.IsValid)
            {
                return View(vm);
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

        #endregion ChangePassword


        #region PrivateMethods

        private async Task<bool> IsUniqueEmail(string email)
        {
            var data = await _userRepository.GetFirstOrDefaultAsync(e => e.Email == email);
            return data == null;
        }

        private bool VerifyPassword(string Password, string HashPassword)
        {
            return BC.EnhancedVerify(Password, HashPassword);
        }

        private bool ValidateUserCredential(Controller controller, LoginVM loginVM, User? user)
        {
            if (user == null)
            {
                controller.TempData["error"] = "Credential Not Matched !";
                return false;
            }

            if (user.RecStatus != Status.Active)
            {
                controller.TempData["error"] = "Please contact to your Administrator";
                return false;
            }

            if (!VerifyPassword(loginVM.Password, user.Password))
            {
                controller.TempData["error"] = "Password Not Matched !";
                return false;
            }

            ;

            return true;
        }

        #endregion PrivateMethods
    }
}