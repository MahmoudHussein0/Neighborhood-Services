using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.BookingImages.Commands;
using Neighborhood.Services.Application.BookingImages.Queries.GetBookingImagesByTypeQuery;
using Neighborhood.Services.Application.BookingImages.Queries.GetBookingImagesQuery;
using Neighborhood.Services.Application.Bookings.Commands.AcceptBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.CancelBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.CompleteBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.ConfirmBookingCommands;
using Neighborhood.Services.Application.Bookings.Commands.CreateBookingCommands;
using Neighborhood.Services.Application.Bookings.Queries.GetAllBookingsQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingByIdQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetMyBookingsQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingsByCustomerQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingsByStatusQuery;
using Neighborhood.Services.Application.Bookings.Queries.GetBookingsByTechnicianQuery;
using Neighborhood.Services.Domain.BookingImages;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.API.Bookings
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---------- Commands ----------

        // POST /api/bookings  (Direct booking)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // POST /api/bookings/{id}/accept  (technician accepts, provides duration)
        [HttpPost("{id:int}/accept")]
        public async Task<IActionResult> Accept(int id, [FromBody] AcceptBookingCommand command)
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

        // GET /api/bookings/mine  (authenticated customer or technician — their own bookings)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _mediator.Send(new GetMyBookingsQuery());
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
