using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsForStaffQuery
{
    public class GetBookingsForStaffQueryHandler
        : IRequestHandler<GetBookingsForStaffQuery, PagedResult<StaffBookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingsForStaffQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<PagedResult<StaffBookingDto>> Handle(GetBookingsForStaffQuery request, CancellationToken cancellationToken)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            return await _bookingRepository.GetBookingsForStaffPagedAsync(request.Status, request.Search, page, pageSize, request.Sort);
        }
    }
}
