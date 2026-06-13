using Mapster;
using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.Commands
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepo , IUnitOfWork unitOfWork)
        {
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {

             var category =  await _categoryRepo.GetByIdAsync(request.Id);

            if (category is null) 
                throw new NotFoundException("Category" , request.Id);

           var isExists =  await _categoryRepo.IsNameExistsAsync(request.NameEn , request.NameAr  ,request.Id);

            if (isExists)
                throw new ValidationException("Category already exists");

            if(!string.IsNullOrWhiteSpace(request.NameEn))
                 category.NameEn = request.NameEn;

            if (!string.IsNullOrWhiteSpace(request.NameAr))
                category.NameAr = request.NameAr;

            if (!string.IsNullOrWhiteSpace(request.Icon))
                 category.Icon = request.Icon;

           await _categoryRepo.UpdateAsync(category);
           await _unitOfWork.SaveChangesAsync(cancellationToken);

            var categoryDto = new CategoryDto()
            {
                Id = category.Id,
                Name =  !string.IsNullOrWhiteSpace(category.NameEn) ? category.NameEn :  category.NameAr, 
                Icon = category.Icon
            };

            return categoryDto;

        }
    }
}
