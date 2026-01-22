using Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistences.Repositories
{
    public interface IGenericRepository<T> where T : IEntity
    {
        Task<long> AddAsync(T entity);

        Task DeleteAsync(long id);

        Task<IEnumerable<T>> GetAllAsync
        (
            Expression<Func<T, object>>[]? includes = null,
            Expression<Func<T, bool>>? predicate = null
        );

        Task<long> UpdateAsync(T entity);

        Task<T?> GetByIdAsync(long id);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task DeleteRangeAsync(IEnumerable<long> ids);

        void Remove(T entity);

        Task<T[]> GetByIdsAsync(long[] ids);
    }
}