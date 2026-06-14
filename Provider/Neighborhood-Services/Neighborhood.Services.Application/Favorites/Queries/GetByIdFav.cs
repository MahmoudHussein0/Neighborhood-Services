using MediatR;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Messages.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.Queries
{
    public class GetByIdFavQDto : IRequest<FavoriteDto>
    {
        public int id { set; get; }
    }
    public class GetByIdFavQHandler : IRequestHandler<GetByIdFavQDto, FavoriteDto>
    {
        private readonly IFavoritesRepository _favrepo;

        public GetByIdFavQHandler(IFavoritesRepository favrepository)
        {
            _favrepo = favrepository;
        }
        public async Task<FavoriteDto> Handle(GetByIdFavQDto request, CancellationToken cancellationToken)
        {
            var item = await _favrepo.GetByIdAsync(request.id);
            if (item == null) return null;
            return new FavoriteDto()
            {
                favoriteId = item.Id,
                userId = item.UserId,
                technicianId = item.TechnicianId,
                addedAt = item.addedAt,
                customerId = item.CustomerId,
                imageURL = item.Technician.TechnicianPhotos.FirstOrDefault()?.PhotoUrl?? "https://www.flaticon.com/free-icon/technician_1085421"
            };

        }
    }
}
