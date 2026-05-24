using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Interfaces
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<List<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task SoftDeleteAsync(int id);        // handles IsDeleted = true directly
        Task SaveChangesAsync();
    }
}
