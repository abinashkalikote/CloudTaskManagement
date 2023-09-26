using App.Web.Providers.Interface;
using App.Web.ViewModel;
using CTM.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using App.Model;
using System.Text.Json;

namespace App.Web.Providers
{
    public class UserProvider : IUserProvider
    {
        public readonly HttpContext _context;

        public UserProvider(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _context = httpContextAccessor.HttpContext;
        }


        public string? GetUsername()
        {
            var user = GetSessionUser();
            return user?.UserName;
        }


        public int? GetUserId()
        {
            var sessionUser = GetSessionUser();
            if (sessionUser != null)
            {
                return sessionUser.UserId;
            }
            return null;
        }

        public bool IsAdmin()
        {
            var sessionUser = GetSessionUser();
            if (sessionUser != null)
            {
                if(sessionUser.IsAdmin == 'Y')
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public SessionUserVM GetSessionUser()
        {
            var userIDClaim = _context.User.Claims.FirstOrDefault(e => e.Type == "SessionUser");
            if(userIDClaim != null)
            {
                var sessionUser = JsonSerializer.Deserialize<SessionUserVM>(userIDClaim.Value);
                return sessionUser;
            }
            return new SessionUserVM();
        }
    }
}
