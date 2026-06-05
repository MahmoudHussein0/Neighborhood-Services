using MediatR;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Categories;

namespace Neighborhood.Services.Application.Categories.Commands
{
    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, int>
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddCategoryCommandHandler(ICategoryRepository categoryRepo , IUnitOfWork unitOfWork)
        {
          _categoryRepo = categoryRepo;
          _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {

           var result =  await _categoryRepo.IsNameExistsAsync(request.NameEn , request.NameAr);

            if (result)
                throw new ValidationException("Category already exists");

            var category = new Category()
            {
                NameEn = request.NameEn!,
                NameAr = request.NameAr!,
                Icon = request.Icon!,
            };

           await  _categoryRepo.AddAsync(category);
           await  _unitOfWork.SaveChangesAsync();
           
           return category.Id;
        }
    }
}
