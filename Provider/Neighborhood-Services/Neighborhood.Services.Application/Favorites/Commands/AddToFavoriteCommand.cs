using MediatR;
using Neighborhood.Services.Application.Conversations.Commands;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.favorites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.Commands
{
    public class AddToFavoriteCommandDto:IRequest<FavoriteDto>
    {
        public int TechnicianId { get; set; }
    }

    public class AddToFavoriteCommandHandler: IRequestHandler<AddToFavoriteCommandDto, FavoriteDto>
    {
        private readonly IFavoritesRepository _favrepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public AddToFavoriteCommandHandler(IFavoritesRepository FavRepo, ICurrentUserService currentUser, IUnitOfWork UnitofWork)
        {
            _favrepo= FavRepo;
            _currentUser=currentUser;
            _unitOfWork = UnitofWork;
            
            
        }

        public async Task<FavoriteDto> Handle(AddToFavoriteCommandDto request, CancellationToken cancellationToken)
        {
            if (_currentUser == null) { return null;}
            Favorite created= new Favorite { UserId=_currentUser.UserId,TechnicianId=request.TechnicianId};
            await _favrepo.AddAsync(created);
            await _unitOfWork.SaveChangesAsync();


            return new FavoriteDto { FavoriteId=created.Id,UserId = created.UserId,TechnicianId=created.TechnicianId,addedAt=created.addedAt};
        }
    }
}
