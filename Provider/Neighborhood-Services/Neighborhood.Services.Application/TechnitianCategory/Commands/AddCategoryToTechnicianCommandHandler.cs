using MediatR;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianCategory.Interface;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.TechnicianCategories;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    internal class AddCategoryToTechnicianCommandHandler : IRequestHandler<AddCategoryToTechnicianCommand, int>
    {

        private readonly ITechnicianCategoryRepository _technicianCategoryRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianRepository _technicianRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddCategoryToTechnicianCommandHandler(ITechnicianCategoryRepository technicianCategoryRepo, ICategoryRepository categoryRepo, ICurrentUserService currentUserService, ITechnicianRepository technicianRepo, IUnitOfWork unitOfWork)
        {
            _technicianCategoryRepo = technicianCategoryRepo;
            _categoryRepo = categoryRepo;
            _currentUserService = currentUserService;
            _technicianRepo = technicianRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(AddCategoryToTechnicianCommand request, CancellationToken cancellationToken)
        {
            string? userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedException("User is unauthorized");

            var technician = await _technicianRepo.GetByUserIdAsync(userId);

            if (technician is null)
                throw new ForbiddenException("User is not a technician");

            var category = await _categoryRepo.GetByIdAsync(request.CategoryId);

            if (category is null)
                throw new NotFoundException("Category not found", request.CategoryId);


            var technicianCategory = (await _technicianCategoryRepo
                .GetByConditionAsync(tc =>
                    tc.TechnicianId == technician.Id &&
                    tc.CategoryId == request.CategoryId))
                .FirstOrDefault();

            if (technicianCategory is not null && !technicianCategory.IsDeleted)
            {
                throw new ValidationException("Technician already has this category.");
            }

            if (technicianCategory is not null && technicianCategory.IsDeleted)
            {
                technicianCategory.IsDeleted = false;
                await _technicianCategoryRepo.UpdateAsync(technicianCategory);
            }
            else
            {
                technicianCategory = new TechnicianCategory
                {
                    CategoryId = request.CategoryId,
                    TechnicianId = technician.Id,
                    IsDeleted = false
                };

                await _technicianCategoryRepo.AddAsync(technicianCategory);
            }

            await _unitOfWork.SaveChangesAsync();

            return technicianCategory.Id;
        }
       
    }
}
