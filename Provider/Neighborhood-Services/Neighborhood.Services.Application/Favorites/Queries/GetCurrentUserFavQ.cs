using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.Queries
{
    public class GetCurrentUserFavQDto : IRequest<List<FavoriteDto>>
    {
      
    }
    public class GetCurrentUserFavQHandler : IRequestHandler<GetCurrentUserFavQDto, List<FavoriteDto>>
    {
        private readonly IFavoritesRepository _favrepo;
        private readonly ICurrentUserService _current;
        private readonly ITechnicianRepository _techRepository;


        public GetCurrentUserFavQHandler(IFavoritesRepository favrepository,
            ICurrentUserService current,
            ITechnicianRepository techRepository)
        {
            _favrepo = favrepository;
            _current= current;
            _techRepository = techRepository;
        }



        public async Task<List<FavoriteDto>> Handle(GetCurrentUserFavQDto request, CancellationToken cancellationToken)
        {
            List<Domain.favorites.Favorite> items = new List<Domain.favorites.Favorite>();
            if (_current == null || _current.UserId == null) { throw new Exception("not authenticated"); }
            items=  await _favrepo.GetByAppUserId(_current.UserId); 

            return items.Select(item => new FavoriteDto
            {
                favoriteId = item.Id,
                userId = item.UserId,
                technicianId = item.TechnicianId,
                technician= _techRepository.GetWithUserDetailsById(item.TechnicianId)?.Result ?? throw new NotFoundException("Couldn't fetch tech data"),


                addedAt = item.addedAt,
                imageURL = item.Technician.TechnicianPhotos.First().PhotoUrl

            }).ToList();
                
        }

    }
    }

