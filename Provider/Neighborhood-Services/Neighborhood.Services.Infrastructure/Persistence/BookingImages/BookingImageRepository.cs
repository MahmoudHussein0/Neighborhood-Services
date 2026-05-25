using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.BookingImages.Interface;
using Neighborhood.Services.Domain.BookingImages;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.BookingImages
{
    public class BookingImageRepository : GenericRepository<BookingImage, int>, IBookingImageRepository
    {
        public BookingImageRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<BookingImage>> GetBookingImagesAsync(int bookingId)
        {
            return await _context.BookingImages
                .Where(bi => bi.BookingId == bookingId && !bi.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingImage>> GetBookingImagesByTypeAsync(int bookingId, BookingImageType type)
        {
            return await _context.BookingImages
                .Where(bi => bi.BookingId == bookingId
                    && bi.Type == type
                    && !bi.IsDeleted)
                .ToListAsync();
        }

    }
}
