using System.Linq.Expressions;
using App.Base.GenericRepository.Interface;
using App.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Base.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();    
        }
        public async Task<bool> CheckIfExistAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().AnyAsync(predicate);

        public async Task<T> FindAsync(long id) => await _context.Set<T>().FindAsync(id);

        public async Task<T> FindOrThrowAsync(long id) => await FindAsync(id) ?? throw new ArgumentNullException(nameof(id));

        public List<T> Get(Expression<Func<T, bool>> predicate) => _context.Set<T>().Where(predicate).ToList();

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= x => true;
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            predicate ??= x => true;
            return await _context.Set<T>().CountAsync(predicate);
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
