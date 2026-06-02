using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Offers.Commands.AcceptOffer;
using Neighborhood.Services.Application.Offers.Commands.CreateOffer;
using Neighborhood.Services.Application.Offers.Commands.RejectOffer;
using Neighborhood.Services.Application.Offers.Commands.Withdraw;
using Neighborhood.Services.Application.Offers.Queries.GetOfferById;
using Neighborhood.Services.Application.Offers.Queries.GetOffersByServiceRequest;
using Neighborhood.Services.Application.Offers.Queries.GetTechnicianOffers;

namespace Neighborhood.Services.API.Offers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OffersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---------- Commands ----------

        // POST /api/offers  (technician submits an offer on a service request)
        // Returns { offerId, warnings[] } — warnings are non-blocking (e.g. outside usual hours)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOfferCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.OfferId }, result);
        }

        // POST /api/offers/{id}/accept  (customer accepts an offer → creates confirmed booking)
        // Returns the new booking id
        [HttpPost("{id:int}/accept")]
        public async Task<IActionResult> Accept(int id)
        {
            var bookingId = await _mediator.Send(new AcceptOfferCommand { OfferId = id });
            return Ok(new { bookingId });
        }

        // POST /api/offers/{id}/reject  (customer rejects a specific offer)
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            await _mediator.Send(new RejectOfferCommand { OfferId = id });
            return NoContent();
        }

        // POST /api/offers/{id}/withdraw  (technician withdraws their own offer)
        [HttpPost("{id:int}/withdraw")]
        public async Task<IActionResult> Withdraw(int id)
        {
            await _mediator.Send(new WithdrawOfferCommand { OfferId = id });
            return NoContent();
        }

        // ---------- Queries ----------

        // GET /api/offers/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetOfferByIdQuery { OfferId = id });
            return Ok(result);
        }

        // GET /api/offers/service-request/{serviceRequestId}
        // Customer sees all offers on their request
        [HttpGet("service-request/{serviceRequestId:int}")]
        public async Task<IActionResult> GetByServiceRequest(int serviceRequestId)
        {
            var result = await _mediator.Send(new GetOffersByServiceRequestQuery { ServiceRequestId = serviceRequestId });
            return Ok(result);
        }

        // GET /api/offers/mine
        // Authenticated technician sees their own offers
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _mediator.Send(new GetTechnicianOffersQuery());
            return Ok(result);
        }
    }
}
