using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.BookingImages.Commands;
using Neighborhood.Services.Application.BookingImages.Queries.GetBookingImagesByTypeQuery;
using Neighborhood.Services.Application.BookingImages.Queries.GetBookingImagesQuery;
using Neighborhood.Services.Application.Bookings.Commands.AcceptQuoteCommands;
using Neighborhood.Services.Application.Bookings.Commands.CancelBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.CompleteBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.ConfirmBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.CreateBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.QuoteBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.RaiseDisputeCommands;
using Neighborhood.Services.Application.Bookings.Commands.RejectQuoteCommands;
using Neighborhood.Services.Application.Bookings.Queries.GetAllBookingsQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingByIdQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetMyBookingsQuery;
using Neighborhood.Services.Application.Bookings.Queries.EstimateBookingPriceQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingsByCustomerQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingsByRecurringQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingsByStatusQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingsByTechnicianQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingsForStaffQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetTechnicianPricingRangeQuery;
using Neighborhood.Services.Application.Bookings.Commands.StaffCancelBookingCommands;
using Neighborhood.Services.Application.Matching.Queries;
using Neighborhood.Services.Domain.BookingImages;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.API.Bookings
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---------- Staff oversight ----------

        // GET /api/bookings/staff?status=&search=&page=1&pageSize=10
        [Authorize(Roles = "Staff")]
        [HttpGet("staff")]
        public async Task<IActionResult> GetForStaff(
            [FromQuery] BookingStatus? status,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetBookingsForStaffQuery
            {
                Status = status,
                Search = search,
                Page = page,
                PageSize = pageSize
            });
            return Ok(result);
        }

        // POST /api/bookings/{id}/staff-cancel  (admin cancel — no refund/reassign, separate from customer/tech cancel)
        [Authorize(Roles = "Staff")]
        [HttpPost("{id:int}/staff-cancel")]
        public async Task<IActionResult> StaffCancel(int id, [FromBody] StaffCancelBookingCommand command)
        {
            command.BookingId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // POST /api/bookings/match
        // "Smart Match" on Find Technician: customer picks category + problem type (+ optional
        // location / note); the matchmaking agent returns the best 1-2 technicians to book.
        [HttpPost("match")]
        public async Task<IActionResult> Match([FromBody] GetTechnicianMatchesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // ---------- Commands ----------

        // POST /api/bookings  (Direct booking)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // POST /api/bookings/{id}/quote  (technician quotes FinalPrice + DurationMinutes)
        [HttpPost("{id:int}/quote")]
        public async Task<IActionResult> Quote(int id, [FromBody] QuoteBookingCommand command)
        {
            command.BookingId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // POST /api/bookings/{id}/accept-quote  (customer accepts the quote -> escrow held -> Confirmed)
        // Optional body { promoCode } discounts the quoted FinalPrice before escrow.
        [HttpPost("{id:int}/accept-quote")]
        public async Task<IActionResult> AcceptQuote(int id, [FromBody] AcceptQuoteCommand? command = null)
        {
            command ??= new AcceptQuoteCommand();
            command.BookingId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // POST /api/bookings/{id}/reject-quote  (customer rejects -> back to Pending so tech can re-quote)
        [HttpPost("{id:int}/reject-quote")]
        public async Task<IActionResult> RejectQuote(int id)
        {
            await _mediator.Send(new RejectQuoteCommand { BookingId = id });
            return NoContent();
        }

        // POST /api/bookings/{id}/dispute  (customer or technician raises a dispute -> Booking Disputed + Dispute record)
        [HttpPost("{id:int}/dispute")]
        public async Task<IActionResult> RaiseDispute(int id, [FromBody] RaiseDisputeCommand command)
        {
            command.BookingId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // POST /api/bookings/{id}/complete  (technician marks completed)
        [HttpPost("{id:int}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            await _mediator.Send(new CompleteBookingCommand { BookingId = id });
            return NoContent();
        }

        // POST /api/bookings/{id}/confirm  (customer confirms job done -> releases escrow)
        [HttpPost("{id:int}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            await _mediator.Send(new ConfirmBookingCommand { BookingId = id });
            return NoContent();
        }

        // POST /api/bookings/{id}/cancel
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelBookingCommand command)
        {
            command.BookingId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // ---------- Queries ----------

        // GET /api/bookings  (admin: all bookings)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllBookingsQuery());
            return Ok(result);
        }

        // GET /api/bookings/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetBookingByIdQuery { BookingId = id });
            return Ok(result);
        }

        // GET /api/bookings/mine?status=Confirmed&page=1&pageSize=10
        // (authenticated customer or technician — their own bookings, paged + optional status filter)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine([FromQuery] BookingStatus? status, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetMyBookingsQuery { Status = status, Search = search, Page = page, PageSize = pageSize });
            return Ok(result);
        }

        // GET /api/bookings/customer/{customerId}  (admin)
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var result = await _mediator.Send(new GetBookingsByCustomerQuery { CustomerId = customerId });
            return Ok(result);
        }

        // GET /api/bookings/technician/{technicianId}
        [HttpGet("technician/{technicianId:int}")]
        public async Task<IActionResult> GetByTechnician(int technicianId)
        {
            var result = await _mediator.Send(new GetBookingsByTechnicianQuery { TechnicianId = technicianId });
            return Ok(result);
        }

        // GET /api/bookings/status/{status}
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(BookingStatus status)
        {
            var result = await _mediator.Send(new GetBookingsByStatusQuery { Status = status });
            return Ok(result);
        }

        // GET /api/bookings/recurring/{recurringBookingId}  (the visits generated by a recurring arrangement)
        [HttpGet("recurring/{recurringBookingId:int}")]
        public async Task<IActionResult> GetByRecurring(int recurringBookingId)
        {
            var result = await _mediator.Send(new GetBookingsByRecurringQuery { RecurringBookingId = recurringBookingId });
            return Ok(result);
        }

        // GET /api/bookings/tech-pricing-range?technicianId=X&problemTypeId=Y
        // Returns the tech's MinPrice/MaxPrice for a single problem type, or 404 if not set.
        // Powers the booking UI: customer sees the tech's range before submitting, tech
        // sees + is constrained to it when quoting.
        [HttpGet("tech-pricing-range")]
        public async Task<IActionResult> GetTechPricingRange([FromQuery] int technicianId, [FromQuery] int problemTypeId)
        {
            var result = await _mediator.Send(new GetTechnicianPricingRangeQuery
            {
                TechnicianId = technicianId,
                ProblemTypeId = problemTypeId
            });
            if (result is null) return NotFound();
            return Ok(result);
        }

        // GET /api/bookings/estimate/{problemTypeId}?region=cairo  (optional, on-demand price estimate)
        [HttpGet("estimate/{problemTypeId:int}")]
        public async Task<IActionResult> Estimate(int problemTypeId, [FromQuery] string? region)
        {
            var price = await _mediator.Send(new EstimateBookingPriceQuery { ProblemTypeId = problemTypeId, Region = region });
            return Ok(new { estimatedPrice = price });
        }

        // ---------- Booking images (nested sub-resource) ----------

        // POST /api/bookings/{id}/images
        [HttpPost("{id:int}/images")]
        public async Task<IActionResult> UploadImage(int id, [FromBody] UploadBookingImageCommand command)
        {
            command.BookingId = id;
            var imageId = await _mediator.Send(command);
            return Ok(new { id = imageId });
        }

        // GET /api/bookings/{id}/images
        [HttpGet("{id:int}/images")]
        public async Task<IActionResult> GetImages(int id)
        {
            var result = await _mediator.Send(new GetBookingImagesQuery { BookingId = id });
            return Ok(result);
        }

        // GET /api/bookings/{id}/images/{type}  (Before | After)
        [HttpGet("{id:int}/images/{type}")]
        public async Task<IActionResult> GetImagesByType(int id, BookingImageType type)
        {
            var result = await _mediator.Send(new GetBookingImagesByTypeQuery { BookingId = id, Type = type });
            return Ok(result);
        }
    }
}
