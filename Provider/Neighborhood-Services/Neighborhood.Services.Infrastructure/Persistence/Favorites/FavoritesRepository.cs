using Neighborhood.Services.Application.Conversations;
using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Application.Favorites;
using Neighborhood.Services.Infrastructure.Shared;
using Neighborhood.Services.Infrastructure.Persistence.Context;

namespace Neighborhood.Services.Infrastructure.Persistence.Favorites
{
    public class FavoritesRepository : GenericRepository<Domain.favorites.Favorite, int>, IFavoritesRepository
    {
        public FavoritesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
