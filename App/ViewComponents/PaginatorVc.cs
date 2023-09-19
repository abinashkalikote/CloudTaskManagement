using App.Base.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Pioneer.Pagination;

namespace App.Web.ViewComponents;

[ViewComponent(Name = "Paginator")]
public class PaginatorVc : ViewComponent
{
    private readonly IPaginatedMetaService _metaService;
    private readonly IHttpContextAccessor _contextAccessor;

    public PaginatorVc(IPaginatedMetaService metaService, IHttpContextAccessor contextAccessor)
    {
        _metaService = metaService;
        _contextAccessor = contextAccessor;
    }
    public async Task<IViewComponentResult> InvokeAsync(PaginationInfo result)
    {
        var path = _contextAccessor.HttpContext.Request.Path;
        var metaData = _metaService.GetMetaData(Convert.ToInt32(result.TotalCount), result.Page,
            result.Limit);
        ViewBag.Path = path;
        ViewBag.TotalCount = result.TotalCount;
        return View(metaData);
    }
}