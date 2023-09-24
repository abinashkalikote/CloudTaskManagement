using App.Web.Providers.Interface;
using App.Web.ViewModel;
using CTM.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using App.Model;

namespace App.Web.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly AppDbContext _db;
        public readonly HttpContext _context;

        private User _currentUser;

        public UserProvider(
            AppDbContext db,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _db = db;
            _context = httpContextAccessor.HttpContext;
        }


        public string? GetUsername()
        {
            var user = GetCurrentUser();
            return user?.FullName;
        }

        private User? GetCurrentUser()
        {
            if (_currentUser == null)
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    _currentUser = null;
                }
                else
                {
                    _currentUser = _db.Users.FirstOrDefault(e => e.Id == userId);
                }
            }
            return _currentUser;
        }

        public int? GetUserId()
        {
            var userIDClaim = _context.User.Claims.FirstOrDefault(e => e.Type == "UserID");
            if (userIDClaim != null && int.TryParse(userIDClaim.Value, out int id))
            {
                return id;
            }
            return null;
        }

        public bool IsAdmin()
        {
            var IsAdmin = _context.User.Claims.FirstOrDefault(e => e.Type == "IsAdmin");
            if (IsAdmin != null)
            {
                if(IsAdmin.Value == "Y")
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
