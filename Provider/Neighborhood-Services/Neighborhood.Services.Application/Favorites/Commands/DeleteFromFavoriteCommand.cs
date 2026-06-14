using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.favorites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.Commands
{
    public class DeleteFromFavoriteCommandDto : IRequest<FavoriteDto>
    {
        public int id { get; set; }
    }

    public class DeleteFromFavoriteCommandHandler : IRequestHandler<DeleteFromFavoriteCommandDto, FavoriteDto>
    {
        private readonly IFavoritesRepository _favrepo;
      //  private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFromFavoriteCommandHandler(IFavoritesRepository FavRepo, ICurrentUserService currentUser, IUnitOfWork UnitofWork)
        {
            _favrepo = FavRepo;
          //  _currentUser = currentUser;
            _unitOfWork = UnitofWork;


        }

        public async Task<FavoriteDto> Handle(DeleteFromFavoriteCommandDto request, CancellationToken cancellationToken)
        {
           // if (_currentUser == null) { return null; }
            Favorite deleted = await _favrepo.GetByIdAsync(request.id)??throw new NotFoundException("No favorite item with given Id");
            await _favrepo.DeleteAsync(deleted.Id);
            await _unitOfWork.SaveChangesAsync();


            return new FavoriteDto { favoriteId = deleted.Id, userId = deleted.UserId, technicianId = deleted.TechnicianId,customerId=deleted.CustomerId, addedAt = deleted.addedAt};
        }
    }
}
