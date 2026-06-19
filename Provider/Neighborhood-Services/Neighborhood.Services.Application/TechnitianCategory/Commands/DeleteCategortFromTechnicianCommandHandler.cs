using MediatR;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianCategory.Interface;
using Neighborhood.Services.Application.TechnitianPricing.Interface;

namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    public class DeleteCategortFromTechnicianCommandHandler : IRequestHandler<DeleteCategortFromTechnicianCommand, bool>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITechnicianCategoryRepository _technicianCategoryRepo;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITechnicianPricingRepository _technicianPricingRepository;

        public DeleteCategortFromTechnicianCommandHandler(ITechnicianCategoryRepository technicianCategoryRepo, ICategoryRepository categoryRepository, ITechnicianPricingRepository technicianPricingRepository, IUnitOfWork unitOfWork)
        {
            _technicianCategoryRepo = technicianCategoryRepo;
            _categoryRepository = categoryRepository;
            _technicianPricingRepository = technicianPricingRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteCategortFromTechnicianCommand request, CancellationToken cancellationToken)
        {

            var technicianCategory = await _technicianCategoryRepo.GetByIdAsync(request.Id);


            // delete all problemTypes related to this Category 
            var category = (await _categoryRepository.GetByConditionAsync(c => c.Id == technicianCategory.CategoryId, "ProblemTypes")).FirstOrDefault();
            var problemTypeIds = category.ProblemTypes.Select(p => p.Id);




            // get problemTypes from TechnicianPricingRepo 
            foreach (var id in problemTypeIds)
            {
                var problemType = (await _technicianPricingRepository.GetByConditionAsync(tp => tp.ProblemTypeId == id)).FirstOrDefault();
                if (problemType is not null)
                    await _technicianPricingRepository.DeleteAsync(problemType.Id);
            }







            if (technicianCategory is null)
                throw new NotFoundException("Pricing", request.Id);
            await _technicianCategoryRepo.DeleteAsync(request.Id);

            return await _unitOfWork.SaveChangesAsync() > 0;

        }
    }
}
