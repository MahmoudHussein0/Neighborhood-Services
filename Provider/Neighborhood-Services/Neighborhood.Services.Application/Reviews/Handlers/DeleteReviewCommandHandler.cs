using MediatR;
using Neighborhood.Services.Application.Reviews.Commands;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
    {
        private readonly IReviewRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteReviewCommandHandler(IReviewRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (review is null)
                throw new Exception($"Review with id {request.Id} not found.");

            review.IsDeleted = true;
            await _repository.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

}
