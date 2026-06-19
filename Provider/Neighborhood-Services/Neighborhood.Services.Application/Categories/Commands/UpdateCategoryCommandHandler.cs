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
        private readonly IBackgroundJobScheduler _jobs;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepo , IUnitOfWork unitOfWork, IBackgroundJobScheduler jobs)
        {
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
            _jobs = jobs;
        }
    

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepo.GetByIdAsync(request.Id);

            if (category is null)
                throw new NotFoundException("Category", request.Id);

            var newNameEn = request.NameEn;
            var newNameAr = request.NameAr;


            var isExists = await _categoryRepo.IsNameExistsAsync(
                newNameEn,
                newNameAr,
                request.Id
            );

            if (isExists)
                throw new ValidationException("Category already exists");

            if (!string.IsNullOrWhiteSpace(newNameEn))
                category.NameEn = newNameEn;

            if (!string.IsNullOrWhiteSpace(newNameAr))
                category.NameAr = newNameAr;

            if (!string.IsNullOrWhiteSpace(request.Icon))
                category.Icon = request.Icon;

            if (!string.IsNullOrWhiteSpace(request.Image))
                category.Image = request.Image;



            await _categoryRepo.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Re-embed this category's vector off the request thread. Fail-open: a queue
            // hiccup must never undo a committed update.
            try { _jobs.EnqueueCategoryIndex(category.Id); } catch { /* RAG sync is best-effort; /reindex is the backstop */ }

            return new CategoryDto
            {
                Id = category.Id, 
                Icon = category.Icon,
                Image = category.Image ,
                NameEn = newNameEn,
                NameAr = newNameAr,
            };
        }
    }
}
