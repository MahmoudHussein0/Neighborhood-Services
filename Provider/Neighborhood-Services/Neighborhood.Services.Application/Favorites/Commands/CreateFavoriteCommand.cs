using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.favorites;
using Neighborhood.Services.Domain.TechnicianPhotos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Favorites.Commands
{
    public class CreateFavoriteCommandDto : IRequest<FavoriteDto>
    {
        public int technicianId { get; set; }
        public string userId { get; set; }
    }

    public class CreateFavoriteCommandHandler : IRequestHandler<CreateFavoriteCommandDto, FavoriteDto>
    {
        private readonly IFavoritesRepository _favrepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITechnicianPhotoRepository _technicianPhotoRepo;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _techRepository;

        public CreateFavoriteCommandHandler(IFavoritesRepository FavRepo,
            IUnitOfWork UnitofWork,
             ITechnicianPhotoRepository TechnicianPhoto,
             ICustomerRepository customerRepository,
             ITechnicianRepository techRepositoy)
        {
            _favrepo = FavRepo;
            _unitOfWork = UnitofWork;
            _technicianPhotoRepo = TechnicianPhoto;
            _customerRepository= customerRepository;
            _techRepository = techRepositoy;

        }

        public async Task<FavoriteDto> Handle(CreateFavoriteCommandDto request, CancellationToken cancellationToken)
        {
            var photos = await _technicianPhotoRepo.GetByTechnicianIdAsync(request.technicianId);



            var chosenPhoto = photos?
                .FirstOrDefault(e => !string.IsNullOrEmpty(e.PhotoUrl)) ?? null ; 
            
            var customer =await _customerRepository.GetByUserIdAsync(request.userId) ?? throw new NotFoundException("No Customer with the given Id");
            var techUserId = _techRepository.GetByIdAsync(request.technicianId);
            

            Favorite created = new Favorite { UserId =request.userId, TechnicianId = request.technicianId,CustomerId=customer.Id };
            await _favrepo.AddAsync(created);
            await _unitOfWork.SaveChangesAsync();


            return new FavoriteDto
            {
                favoriteId = created.Id,
                userId = created.UserId,
                technicianId = created.TechnicianId,
                customerId = created.CustomerId,
                addedAt = created.addedAt,
                imageURL = chosenPhoto != null ? chosenPhoto.PhotoUrl : "https://cdn-icons-png.flaticon.com/512/1085/1085421.png"
            };
        }
    }
}
