using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianCategory.Interface;

namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    public class RemoveCategortFromTechnicianCommandHandler : IRequestHandler<RemoveCategortFromTechnicianCommand, bool>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITechnicianCategoryRepository _technicianCategoryRepo;


        public RemoveCategortFromTechnicianCommandHandler(ITechnicianCategoryRepository technicianCategoryRepo, IUnitOfWork unitOfWork)
        {
            _technicianCategoryRepo = technicianCategoryRepo;
            _unitOfWork = unitOfWork;
        }
        public async  Task<bool> Handle(RemoveCategortFromTechnicianCommand request, CancellationToken cancellationToken)
        {
            var techCategories = await _technicianCategoryRepo.GetByConditionAsync(TC => TC.TechnicianId == request.TechnicianId && TC.CategoryId == request.CategoryId);
            var techCategory = techCategories.FirstOrDefault();
            
            if (techCategory is null)
                throw new NotFoundException("TechnicianCategory" , techCategory.Id);

                   await  _technicianCategoryRepo.DeleteAsync(techCategory.Id);
            return await _unitOfWork.SaveChangesAsync() > 0; 

        }
    }
}
