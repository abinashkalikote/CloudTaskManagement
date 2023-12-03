using App.Base.DataContext.Interface;
using App.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Base.DataContext
{
    public class Uow : IUow
    {
        private readonly AppDbContext _context;

        public Uow(AppDbContext context)
        {
            _context = context;
        }

        public void Commit() => _context.SaveChanges();

        public async Task CommitAsync() => await _context.SaveChangesAsync();

        public async Task CreateAsync<T>(T entity) => await _context.AddAsync(entity);

        public Task CreateRangeAsync<T>(IEnumerable<T> list) where T : class => _context.AddRangeAsync(list);

        public void Remove<T>(T entity) => _context.Remove(entity);

        public void RemoveRange<T>(IEnumerable<T> list) where T : class => _context.RemoveRange(list);

        public void Update<T>(T entity) => _context.Update(entity);

        public void UpdateRange<T>(IEnumerable<T> list) where T : class => _context.UpdateRange(list);
    }
}
