using App.Web.Providers.Interface;
using App.Web.ViewModel;
using CTM.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace App.Web.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly AppDbContext _db;
        public readonly HttpContext _context;

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
            int id = 0;
            var userIDClaim = _context.User.Claims.FirstOrDefault(e => e.Type == "UserID");
            if (userIDClaim != null && int.TryParse(userIDClaim.Value, out id))
            {
                var user = _db.Users.FirstOrDefault(e => e.Id == id);
                if (user != null)
                {
                    return user.FullName;
                }
            }


            return null;
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
    }
}
