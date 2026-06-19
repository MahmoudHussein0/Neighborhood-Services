using Mapster;
using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using System.Reflection.Metadata.Ecma335;


namespace Neighborhood.Services.Application.Categories.Commands
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobScheduler _jobs;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepo , IUnitOfWork unitOfWork, IBackgroundJobScheduler jobs)
        {
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
            _jobs = jobs;
        }
        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepo.GetByIdAsync(request.Id);

            if (category is null) throw new NotFoundException("Category" , request.Id);

            await  _categoryRepo.DeleteAsync(category.Id);
            var deleted = await _unitOfWork.SaveChangesAsync() > 0 ;

            // Drop this category's vector from the RAG index off the request thread. Fail-open:
            // a queue hiccup must never undo a committed delete.
            if (deleted)
                try { _jobs.EnqueueCategoryRemoval(category.Id); } catch { /* RAG sync is best-effort; /reindex is the backstop */ }

            return deleted;
        }
    }
}
