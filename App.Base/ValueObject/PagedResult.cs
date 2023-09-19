using System.Collections;
using System.Collections.Generic;

namespace App.Base.ValueObject;

public class PagedResult<T> : IEnumerable<T> where T : class
{
    public readonly IEnumerable<T> Collection;
    public readonly int TotalCollectionSize;
    public readonly int CurrentPage;
    public readonly int Limit;

    public PaginationInfo Info => new PaginationInfo(TotalCollectionSize, CurrentPage, TotalCollectionSize == 0 ? 0 : Limit);

    public PagedResult(IEnumerable<T> collection, int totalCollectionSize, int currentPage, int limit)
    {
        Collection = collection;
        TotalCollectionSize = totalCollectionSize;
        CurrentPage = currentPage;
        Limit = limit;
    }

    public IEnumerator<T> GetEnumerator()
        => Collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class PaginationInfo
{
    public long TotalCount { get; }
    public int Page { get; }
    public int Limit { get; }

    public PaginationInfo(long totalCount, int page, int limit)
    {
        TotalCount = totalCount;
        Page = page;
        Limit = limit;
    }
}