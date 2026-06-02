using MediatR;
using Neighborhood.Services.Application.Favorites.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.Queries
{
    
    public class GetAllFavQQHandler : IRequestHandler<IRequest<List<FavoriteDto>>, List<FavoriteDto>>
    {
        private readonly IFavoritesRepository _favrepo;

        public GetAllFavQQHandler(IFavoritesRepository favrepository)
        {
            _favrepo = favrepository;
        }
        public async Task<List<FavoriteDto>> Handle(IRequest<List<FavoriteDto>> request, CancellationToken cancellationToken)
        {
            var items = await _favrepo.GetAllAsync();
            if (items.Count == 0) { return null; }
            return items.Select(item => new FavoriteDto
            {
                FavoriteId = item.Id,
                UserId = item.UserId,
                TechnicianId = item.TechnicianId,
                addedAt = item.addedAt

            })
                .ToList();
        }
    }
}
