using Neighborhood.Services.Application.Reviews.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Services
{
    public interface IReviewService
    {
        Task CreateReviewAsync(CreateReviewDto dto);

        Task<List<ReviewDto>> GetAllAsync();

        Task<ReviewDto?> GetByIdAsync(int id);

        Task DeleteAsync(int id);
    }
}
