using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Application.Technicians.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.Queries
{
    public class GetAllFavQDto : IRequest<List<FavoriteDto>>
    {

    }
    public class GetAllFavQHandler : IRequestHandler<GetAllFavQDto, List<FavoriteDto>>
    {
        private readonly IFavoritesRepository _favrepo;
        public readonly ITechnicianRepository _techrepo;

        public GetAllFavQHandler(IFavoritesRepository favrepository,
           ITechnicianRepository techrepo )
        {
            _favrepo = favrepository;
            _techrepo=techrepo;
        }
        public async Task<List<FavoriteDto>> Handle(GetAllFavQDto request, CancellationToken cancellationToken)
        {
            var items = await _favrepo.GetAllAsync();
            if (items == null) { return null; }
            return items
                .Select(item => new FavoriteDto
                {
                    favoriteId = item.Id,
                    userId = item.UserId,
                    technician= _techrepo.GetWithUserDetailsById(item.TechnicianId)?.Result?? throw new NotFoundException("Couldn't fetch tech data"),
                    customerId = item.CustomerId,
                    technicianId = item.TechnicianId,
                    addedAt = item.addedAt

                })
                .ToList();
        }
    }
}
