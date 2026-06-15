using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Domain.favorites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites
{
    public interface IFavoritesRepository : IGenericRepository<Domain.favorites.Favorite, int>
    {
        public Task<List<Domain.favorites.Favorite>> GetByAppUserId(string userId);

        public Task<bool> CheckIfExists(string userId, int TechnicianId);

        //public  Task<IReadOnlyList<Favorite>> DeleteAsync(int delId);

        //public Task<IReadOnlyList<FavoriteDto>> GetAllDetailsAsync();


    }
}
