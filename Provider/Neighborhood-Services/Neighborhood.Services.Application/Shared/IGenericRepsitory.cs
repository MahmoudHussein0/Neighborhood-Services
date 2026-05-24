using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Neighborhood.Services.Application.Shared
{
    public interface IGenericRepository<T, Tkey> where T : class
    {
        Task<T> GetByIdAsync(Tkey id);
        Task<IReadOnlyList<T>> GetAllAsync();
        IQueryable<T> Table { get; }
        Task AddAsync(T entity);
        Task DeleteAsync(Tkey id);
        Task UpdateAsync(T entity);

        Task<IEnumerable<T>> GetByConditionAsync(
            Expression<Func<T, bool>> expression,
            string? includeProperties = null,
            bool trackChanges = true);
    }
}
