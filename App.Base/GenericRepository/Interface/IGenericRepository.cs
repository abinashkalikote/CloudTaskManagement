using System.Linq.Expressions;

namespace App.Base.GenericRepository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);
        List<T> Get(Expression<Func<T, bool>> predicate);
        Task<T> GetItemAsync(Expression<Func<T, bool>> predicate);
        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindAsync(long id);
        Task<bool> CheckIfExistAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindOrThrowAsync(long id);
        IQueryable<T> GetQueryable();

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> expression);
    }
}
