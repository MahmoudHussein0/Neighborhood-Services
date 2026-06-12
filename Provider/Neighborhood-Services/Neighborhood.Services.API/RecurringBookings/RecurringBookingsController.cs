using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.RecurringBookings.Commands.ApproveRecurring;
using Neighborhood.Services.Application.RecurringBookings.Commands.CancelRecurring;
using Neighborhood.Services.Application.RecurringBookings.Commands.CreateRecurring;
using Neighborhood.Services.Application.RecurringBookings.Commands.PauseRecurring;
using Neighborhood.Services.Application.RecurringBookings.Commands.RejectRecurringPrice;
using Neighborhood.Services.Application.RecurringBookings.Commands.ResumeRecurring;
using Neighborhood.Services.Application.RecurringBookings.Commands.SetRecurringPrice;
using Neighborhood.Services.Application.RecurringBookings.Commands.UpdateRecurring;
using Neighborhood.Services.Application.RecurringBookings.Queries.GetMyRecurringBookingsQuery;
using Neighborhood.Services.Application.RecurringBookings.Queries.GetRecurringBookingByIdQuery;
using Neighborhood.Services.Domain.RecurringBookings;

namespace Neighborhood.Services.API.RecurringBookings
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecurringBookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecurringBookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---------- Commands ----------

        // POST /api/recurringbookings  (customer creates a recurring arrangement)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRecurringBookingCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT /api/recurringbookings/{id}  (customer updates terms → resets to PendingApproval)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRecurringBookingCommand command)
        {
            command.RecurringBookingId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // POST /api/recurringbookings/{id}/set-price  (technician sets their price)
        [HttpPost("{id:int}/set-price")]
        public async Task<IActionResult> SetPrice(int id, [FromBody] SetRecurringPriceCommand command)
        {
            command.RecurringBookingId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // POST /api/recurringbookings/{id}/approve  (customer approves the price → Active)
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            await _mediator.Send(new ApproveRecurringPriceCommand { RecurringBookingId = id });
            return NoContent();
        }

        // POST /api/recurringbookings/{id}/reject-price  (customer rejects price → back to PendingApproval)
        [HttpPost("{id:int}/reject-price")]
        public async Task<IActionResult> RejectPrice(int id)
        {
            await _mediator.Send(new RejectRecurringPriceCommand { RecurringBookingId = id });
            return NoContent();
        }

        // POST /api/recurringbookings/{id}/pause  (customer pauses)
        [HttpPost("{id:int}/pause")]
        public async Task<IActionResult> Pause(int id)
        {
            await _mediator.Send(new PauseRecurringBookingCommand { RecurringBookingId = id });
            return NoContent();
        }

        // POST /api/recurringbookings/{id}/resume  (customer resumes)
        [HttpPost("{id:int}/resume")]
        public async Task<IActionResult> Resume(int id)
        {
            await _mediator.Send(new ResumeRecurringBookingCommand { RecurringBookingId = id });
            return NoContent();
        }

        // POST /api/recurringbookings/{id}/cancel
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelRecurringBookingCommand command)
        {
            command.RecurringBookingId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // ---------- Queries ----------

        // GET /api/recurringbookings/mine?status=Active&search=cairo&page=1&pageSize=10
        // (authenticated customer or technician — paged + optional filter/search)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine(
            [FromQuery] RecurringBookingStatus? status,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetMyRecurringBookingsQuery
            {
                Status = status,
                Search = search,
                Page = page,
                PageSize = pageSize
            });
            return Ok(result);
        }

        // GET /api/recurringbookings/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetRecurringBookingByIdQuery { RecurringBookingId = id });
            return Ok(result);
        }
    }
}
