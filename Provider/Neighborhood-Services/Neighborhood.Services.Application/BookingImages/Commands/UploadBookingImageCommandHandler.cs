using MediatR;
using Neighborhood.Services.Application.BookingImages.Interface;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.BookingImages;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.BookingImages.Commands
{
    public class UploadBookingImageCommandHandler : IRequestHandler<UploadBookingImageCommand, int>
    {
        private readonly IBookingImageRepository _bookingImageRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UploadBookingImageCommandHandler(
            IBookingImageRepository bookingImageRepository,
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _bookingImageRepository = bookingImageRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(UploadBookingImageCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            // Only the customer or technician on this booking can upload images
            if (booking.Customer.ApplicationUserId != userId && booking.Technician.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this booking.");

            if (!IsValidHttpUrl(request.ImageUrl))
                throw new BadRequestException("ImageUrl must be a valid absolute http/https URL.");

            var image = new BookingImage
            {
                BookingId = request.BookingId,
                ImageUrl = request.ImageUrl,
                Type = request.Type,
                UploadedBy = userId,
                UploadedAt = DateTime.UtcNow
            };

            await _bookingImageRepository.AddAsync(image);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return image.Id;
        }

        private static bool IsValidHttpUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
