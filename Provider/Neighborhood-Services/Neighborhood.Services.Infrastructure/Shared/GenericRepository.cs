using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Shared
{
   
        public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
        {
        protected readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }


         public IQueryable<T> Table => _context.Set<T>().AsNoTracking();


            public virtual async Task AddAsync(T entity)
            {
                await _context.Set<T>().AddAsync(entity);
            }



        public virtual async Task DeleteAsync(TKey id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity is BaseEntity<TKey> baseEntity)
            {
                baseEntity.IsDeleted = true;
                _context.Set<T>().Update(entity);
            }
        }


        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
            {
                return await _context.Set<T>().AsNoTracking().ToListAsync();
            }



            public virtual async Task<T> GetByIdAsync(TKey id)
            {
                return await _context.Set<T>().FindAsync(id);
            }


            public virtual Task UpdateAsync(T entity)
            {
                _context.Set<T>().Update(entity);
                return Task.CompletedTask;
            }


            public virtual async Task<IEnumerable<T>> GetByConditionAsync(
                Expression<Func<T, bool>> expression,
                string? includeProperties = null,
                bool trackChanges = true)
            {
                // 1. Apply the condition (WHERE clause)
                IQueryable<T> query = _context.Set<T>().Where(expression);
                // 2. Apply Includes if any are provided
                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    // Split by comma in case there are multiple includes (e.g., "CartItems,CartItems.Product")
                    foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty);
                    }
                }
                // 3. Execute the query and return the result
                return await query.ToListAsync();
            }
        }
    }

