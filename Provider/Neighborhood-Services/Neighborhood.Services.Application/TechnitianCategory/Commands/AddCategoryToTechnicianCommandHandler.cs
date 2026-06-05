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
        private readonly ITechnicianRepository _technicianRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddCategoryToTechnicianCommandHandler(ITechnicianCategoryRepository technicianCategoryRepo , ICategoryRepository categoryRepo ,  ITechnicianRepository technicianRepo, IUnitOfWork unitOfWork)
        {
           _technicianCategoryRepo = technicianCategoryRepo;
           _categoryRepo = categoryRepo;
            _technicianRepo = technicianRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(AddCategoryToTechnicianCommand request, CancellationToken cancellationToken)
        {
            var technician = await _technicianRepo.GetByIdAsync(request.TechnicianId);
            var category = await _categoryRepo.GetByIdAsync(request.CategoryId);


            if (technician is null)
                throw new NotFoundException("Technician", request.TechnicianId);


            if (category is null)
                throw new NotFoundException("Problem", request.CategoryId);

            if (await _technicianCategoryRepo.IsExists(request.TechnicianId, request.CategoryId))
            {
                throw new ValidationException("Technician already has this category.");}


            var techCategory = new TechnicianCategory()
            {
                CategoryId = request.CategoryId,
                TechnicianId = request.TechnicianId,
            };


            await   _technicianCategoryRepo.AddAsync(techCategory);
            await _unitOfWork.SaveChangesAsync();

            return techCategory.Id;

        }
    }
}
