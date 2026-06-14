using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.QA.Interface;
using Neighborhood.Services.Application.Reviews.Commands;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Reviews;


namespace Neighborhood.Services.Application.Reviews.Handlers
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
    {
        private readonly IReviewRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingRepository _bookingRepository;
        private readonly IQaAgent _qaAgent;
        private readonly ICurrentUserService _currentUser;

        public CreateReviewCommandHandler(IReviewRepository repository, IUnitOfWork unitOfWork, IBookingRepository bookingRepository , IQaAgent qaAgent , ICurrentUserService currentUserService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
           _qaAgent = qaAgent;
            _currentUser = currentUserService;
        }

        public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            if (request.Rating < 1 || request.Rating > 5)
                throw new BadRequestException("Rating must be between 1 and 5.");

            if (string.IsNullOrWhiteSpace(request.Comment))
                throw new BadRequestException("A comment is required.");

            var reviewerId = _currentUser.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId)
                ?? throw new NotFoundException(nameof(Booking), request.BookingId);

            // A review only makes sense once the cycle is fully closed: the customer has
            // confirmed the completed work and the escrow was released. This prevents
            // review-before-dispute sequencing and stops reviews on jobs that aren't really done.
            if (booking.Status != BookingStatus.Completed || !booking.ClientConfirmed)
                throw new BadRequestException("You can only review a booking after the customer confirms completion.");

            var customerUserId = booking.Customer.ApplicationUserId;
            var technicianUserId = booking.Technician.ApplicationUserId;

            // Derive direction from WHO is calling — the auth token is the source of truth.
            // The reviewee is whichever party the reviewer is not.
            ReviewType direction;
            string revieweeId;
            if (reviewerId == customerUserId)
            {
                direction = ReviewType.CustomerToTechnician;
                revieweeId = technicianUserId;
            }
            else if (reviewerId == technicianUserId)
            {
                direction = ReviewType.TechnicianToCustomer;
                revieweeId = customerUserId;
            }
            else
            {
                throw new ForbiddenException("You don't have access to this booking.");
            }

            if (await _repository.ExistsByDirectionAsync(request.BookingId, direction, cancellationToken))
                throw new ConflictException("You have already reviewed this booking.");

            var review = new Review
            {
                BookingId = request.BookingId,
                ReviewerId = reviewerId,
                RevieweeId = revieweeId,
                ReviewType = direction,
                Rating = request.Rating,
                Comment = request.Comment.Trim(),
                Status = ReviewStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // QA moderation is best-effort: the review is already persisted, so a failing/slow
            // AI call (bad key, rate limit, malformed JSON, duplicate analysis) must NOT fail the
            // submission. Fail open — the AI call logs its own failures via AgentLog, and staff can
            // still moderate manually. Re-analysis can be triggered later if needed.
            try
            {
                await _qaAgent.AnalyzeReviewAsync(request.Comment, review.Id);
            }
            catch
            {
                // Swallow — analysis is non-critical to creating the review.
            }

            return ReviewMapper.MapToDto(review);
        }
    }
}
