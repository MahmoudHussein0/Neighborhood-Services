using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Favorites.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.favorites;
using Neighborhood.Services.Domain.TechnicianPhotos;
using Neighborhood.Services.Domain.Technicians;


namespace Neighborhood.Services.Application.Favorites.Commands
{
    public class AddToFavoriteCommandDto:IRequest<FavoriteDto>
    {
        public int technicianId { get; set; }
    }

    public class AddToFavoriteCommandHandler: IRequestHandler<AddToFavoriteCommandDto, FavoriteDto>
    {
        private readonly IFavoritesRepository _favrepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITechnicianPhotoRepository _technicianPhotoRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ITechnicianRepository _technicianRepo;


        public AddToFavoriteCommandHandler(IFavoritesRepository FavRepo, 
            ICurrentUserService currentUser, 
            IUnitOfWork UnitofWork,
             ITechnicianPhotoRepository TechnicianPhoto,
             ICustomerRepository customerRepo,
             ITechnicianRepository technicianRepo)
        {
            _favrepo= FavRepo;
            _currentUser=currentUser;
            _unitOfWork = UnitofWork;
            _technicianPhotoRepo = TechnicianPhoto;
            _customerRepo = customerRepo;
            _technicianRepo = technicianRepo;


        }

        public async Task<FavoriteDto> Handle(AddToFavoriteCommandDto request, CancellationToken cancellationToken)
        {
            var photos = await _technicianPhotoRepo.GetByTechnicianIdAsync(request.technicianId);

            
            if (_currentUser == null || _currentUser.UserId==null) { return null!;}
            List<TechnicianPhoto> addedPhotos = new List<TechnicianPhoto>()
           {await _technicianPhotoRepo.GetByIdAsync(request.technicianId) ??
           throw new NotFoundException("No Technician with the given Id")};

            var exists = await _favrepo.CheckIfExists(_currentUser.UserId, request.technicianId);
            if (exists)
            { throw new Exception("Item is already in favorites"); }

            TechnicianPhoto? chosenPhoto = addedPhotos.FirstOrDefault(e => e.PhotoUrl != null) ?? null ;

            var customer = await _customerRepo.GetByUserIdAsync(_currentUser.UserId)?? throw new NotFoundException("Invalid Customer Id");
            

            Favorite created = new Favorite { UserId=_currentUser.UserId,TechnicianId=request.technicianId,CustomerId=customer.Id};
            await _favrepo.AddAsync(created);
            await _unitOfWork.SaveChangesAsync();

            var techDto = await _technicianRepo.GetWithUserDetailsById(request.technicianId)?? throw new NotFoundException("Invalid Tech Id"); 


            return new FavoriteDto
            {
                favoriteId = created.Id,
                userId = created.UserId,
                technicianId = created.TechnicianId,
                customerId= created.CustomerId,
                technician= techDto,
                addedAt = created.addedAt,

                imageURL = chosenPhoto != null ? chosenPhoto.PhotoUrl : "https://cdn-icons-png.flaticon.com/512/1085/1085421.png"
            };
        }
    }
}

