using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Base.DataContext.Interface
{
    public interface IUow
    {
        void Commit();
        Task CommitAsync();
        Task CreateAsync<T>(T entity);
        Task CreateRangeAsync<T>(IEnumerable<T> list) where T : class;
        void Update<T>(T entity);
        void UpdateRange<T>(IEnumerable<T> list) where T : class;
        void Remove<T>(T entity);
        void RemoveRange<T>(IEnumerable<T> list) where T : class;
    }
}
