using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianCategory.Interface;
using Neighborhood.Services.Domain.TechnicianCategories;

namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    internal class AddCategoryToTechnicianCommandHandler : IRequestHandler<AddCategoryToTechnicianCommand, int>
    {

        private readonly ITechnicianCategoryRepository _technicianCategoryRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddCategoryToTechnicianCommandHandler(ITechnicianCategoryRepository technicianCategoryRepo, IUnitOfWork unitOfWork)
        {
           _technicianCategoryRepo = technicianCategoryRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(AddCategoryToTechnicianCommand request, CancellationToken cancellationToken)
        {

            if (await _technicianCategoryRepo.IsExists(request.TechnicianId, request.CategoryId))
            {
                throw new ValidationException(new Dictionary<string, string[]>
                    {{ "CategoryId", new[] { "Technician already has this category." }}});}


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
