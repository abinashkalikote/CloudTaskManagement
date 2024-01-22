using App.Base.Providers.Interfaces;

namespace App.Web.Providers.Implementation;

public class HostProvider : IHostProvider
{
    private readonly HttpContext? _context;
    public HostProvider(IHttpContextAccessor httpContextAccessor)
    {
        _context = httpContextAccessor.HttpContext;
    }
    public string GetHost()
    {
        // Get the current HttpContext
        var scheme = _context.Request.Scheme;
        var host = _context.Request.Host.Host;
        var port = _context.Request.Host.Port;
        var baseUrl = $"{scheme}://{host}:{port}";
        return baseUrl;
    }
}