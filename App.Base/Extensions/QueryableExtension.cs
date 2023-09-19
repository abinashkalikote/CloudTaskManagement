using System.Linq;
using System.Threading.Tasks;
using App.Base.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace App.Base.Extensions;

public static class QueryableExtension
{
    public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> queryable, int page = 1, int limit = 10)
        where T : class
    {
        return new PagedResult<T>(await queryable.Skip(((page - 1) * limit)).Take(limit).ToListAsync(), await queryable.CountAsync(), page, limit);
    }
}