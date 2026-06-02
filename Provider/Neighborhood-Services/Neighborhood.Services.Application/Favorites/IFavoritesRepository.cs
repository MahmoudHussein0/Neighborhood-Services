using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Escrows;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites
{
    public interface IFavoritesRepository : IGenericRepository<Domain.favorites.Favorite, int>
    {
    }
}
