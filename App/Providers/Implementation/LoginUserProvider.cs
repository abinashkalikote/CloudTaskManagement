using System.Text.Json;
using App.Base.Providers.Interfaces;
using App.Base.ValueObject;

namespace App.Web.Providers.Implementation;

public class LoginUserProvider : ILoginUserProvider
{
    private readonly HttpContext? _context;

    public LoginUserProvider(IHttpContextAccessor httpContextAccessor )
    {
        _context = httpContextAccessor.HttpContext;
    }
    
    public string? GetUsername()
    {
        var user = GetSessionUser();
        return user?.UserName;
    }


    public long GetUserId()
    {
        var sessionUser = GetSessionUser();
        return sessionUser.UserId;
    }

    public bool IsAdmin()
    {
        var sessionUser = GetSessionUser();
        if (sessionUser == null) return false;
        return sessionUser.IsAdmin == 'Y';
    }

    public SessionUser? GetSessionUser()
    {
        var userIdClaim = _context?.User.Claims.FirstOrDefault(e => e.Type == "SessionUser");
        if (userIdClaim == null) return new SessionUser();
        var sessionUser = JsonSerializer.Deserialize<SessionUser>(userIdClaim.Value);
        return sessionUser;
    }
}