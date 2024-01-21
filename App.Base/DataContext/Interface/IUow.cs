using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace App.Base.DataContext.Interface
{
    public interface IUow
    {
        void Commit();
        Task CommitAsync();
        Task<T> CreateAsync<T>(T entity);
        Task CreateRangeAsync<T>(IEnumerable<T> list) where T : class;
        void Update<T>(T entity);
        void UpdateRange<T>(IEnumerable<T> list) where T : class;
        void Remove<T>(T entity);
        void RemoveRange<T>(IEnumerable<T> list) where T : class;
    }
}
