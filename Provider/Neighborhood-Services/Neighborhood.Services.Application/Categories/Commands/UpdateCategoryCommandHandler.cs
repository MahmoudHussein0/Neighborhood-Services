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

           var isExists =  await _categoryRepo.IsNameExistsAsync(request.Name);

            if(isExists && category.Name != request.Name)
                throw new ValidationException(new Dictionary<string, string[]>
                {{"Name", new[] { "Category already exists"} }});


            category.Name = request.Name;
            category.Icon = request.Icon;

           await _categoryRepo.UpdateAsync(category);
           await _unitOfWork.SaveChangesAsync();

          return   category.Adapt<CategoryDto>();

        }
    }
}
