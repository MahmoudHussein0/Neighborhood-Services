using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
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

        public AddCategoryCommandHandler(ICategoryRepository categoryRepo, IUnitOfWork unitOfWork)
        {
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {

            var category = (await _categoryRepo.GetByConditionAsync(c => c.NameEn.Trim().ToLower() == request.NameEn.Trim().ToLower() || c.NameAr.Trim() == request.NameAr.Trim())).FirstOrDefault();

            if (category is not null &&  !category.IsDeleted)
                throw new ValidationException("Category already exists");


            if (category  is not null && category.IsDeleted )
            {

                category.NameAr = request.NameAr;
                category.NameEn = request.NameEn;
                category.Icon = request.Icon;
                category.Image = request?.Image;
                category.IsDeleted = false;
                await _categoryRepo.UpdateAsync(category);


            }
            else
            {
           category = new Category()
            {
                Icon = request.Icon!,
                NameEn = request.NameEn,
                NameAr = request.NameAr,
                Image = request?.Image
            };

            await _categoryRepo.AddAsync(category);
            }
            await _unitOfWork.SaveChangesAsync();



            return category.Id;
        }
    }
}
