using MediatR;
using Neighborhood.Services.Application.BookingImages.DTOs;
using Neighborhood.Services.Application.BookingImages.Interface;

namespace Neighborhood.Services.Application.BookingImages.Queries.GetBookingImagesByTypeQuery
{
    public class GetBookingImagesByTypeQueryHandler : IRequestHandler<GetBookingImagesByTypeQuery, IEnumerable<BookingImageDto>>
    {
        private readonly IBookingImageRepository _bookingImageRepository;

        public GetBookingImagesByTypeQueryHandler(IBookingImageRepository bookingImageRepository)
        {
            _bookingImageRepository = bookingImageRepository;
        }

        public async Task<IEnumerable<BookingImageDto>> Handle(GetBookingImagesByTypeQuery request, CancellationToken cancellationToken)
        {
            var images = await _bookingImageRepository.GetBookingImagesByTypeAsync(request.BookingId, request.Type);

            return images.Select(i => new BookingImageDto
            {
                Id = i.Id,
                BookingId = i.BookingId,
                ImageUrl = i.ImageUrl,
                Type = i.Type,
                UploadedBy = i.UploadedBy,
                UploadedAt = i.UploadedAt
            });
        }
    }
}
