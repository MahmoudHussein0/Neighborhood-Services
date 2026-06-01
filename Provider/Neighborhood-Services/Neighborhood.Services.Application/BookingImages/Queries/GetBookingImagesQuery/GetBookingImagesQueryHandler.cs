using MediatR;
using Neighborhood.Services.Application.BookingImages.DTOs;
using Neighborhood.Services.Application.BookingImages.Interface;

namespace Neighborhood.Services.Application.BookingImages.Queries.GetBookingImagesQuery
{
    public class GetBookingImagesQueryHandler : IRequestHandler<GetBookingImagesQuery, IEnumerable<BookingImageDto>>
    {
        private readonly IBookingImageRepository _bookingImageRepository;

        public GetBookingImagesQueryHandler(IBookingImageRepository bookingImageRepository)
        {
            _bookingImageRepository = bookingImageRepository;
        }

        public async Task<IEnumerable<BookingImageDto>> Handle(GetBookingImagesQuery request, CancellationToken cancellationToken)
        {
            var images = await _bookingImageRepository.GetBookingImagesAsync(request.BookingId);

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
