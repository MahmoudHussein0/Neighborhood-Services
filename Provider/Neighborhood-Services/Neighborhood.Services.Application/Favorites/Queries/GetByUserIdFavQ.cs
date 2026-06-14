using MediatR;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Neighborhood.Services.Application.Favorites.Queries
{
    public class GetByUserIdFavQDto : IRequest<List<FavoriteDto>>
    {
        public string userId { set; get; }
    }
    public class GetByUserIdFavQHandler : IRequestHandler<GetByUserIdFavQDto, List<FavoriteDto>>
    {
        private readonly IFavoritesRepository _favrepo;

        public GetByUserIdFavQHandler(IFavoritesRepository favrepository
           )
        {
            _favrepo = favrepository;
            
        }



        public async Task<List<FavoriteDto>> Handle(GetByUserIdFavQDto request, CancellationToken cancellationToken)
        {
            //this method in repo includes favorites with technicians and technician photo
            List<Domain.favorites.Favorite> items = new List<Domain.favorites.Favorite>();
            items = await _favrepo.GetByAppUserId(request.userId);

            return items.Select(item => new FavoriteDto
            {
                favoriteId = item.Id,
                userId = item.UserId,
                technicianName = item.User.FullName,
                technicianId = item.TechnicianId,
                customerId = item.CustomerId,
                addedAt = item.addedAt,
                imageURL = item.Technician.TechnicianPhotos.FirstOrDefault()?.PhotoUrl ?? "https://www.flaticon.com/free-icon/technician_1085421"

            }).ToList();

        }

    }
}
