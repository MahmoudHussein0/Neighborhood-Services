using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.BookingImages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.BookingImages.Interface
{
    public interface IBookingImageRepository : IGenericRepository<BookingImage, int>
    {
        Task<IEnumerable<BookingImage>> GetBookingImagesAsync(int bookingId);
        Task<IEnumerable<BookingImage>> GetBookingImagesByTypeAsync(int bookingId, BookingImageType type);
    }
}
