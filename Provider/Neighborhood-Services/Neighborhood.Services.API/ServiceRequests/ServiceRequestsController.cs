using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.ServiceRequests.Commands.CreateService;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetMyServiceRequestsQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetOpenServiceRequestsQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestByIdQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestsByCustomerQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestWithOffersQuery;

namespace Neighborhood.Services.API.ServiceRequests
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServiceRequestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---------- Commands ----------

        // POST /api/servicerequests  (customer posts a new request for the bidding flow)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequestCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // ---------- Queries ----------

        // GET /api/servicerequests/mine  (authenticated customer — their own requests)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _mediator.Send(new GetMyServiceRequestsQuery());
            return Ok(result);
        }

        // GET /api/servicerequests/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetServiceRequestByIdQuery { ServiceRequestId = id });
            return Ok(result);
        }

        // GET /api/servicerequests/{id}/with-offers  (customer sees all offers on their request)
        [HttpGet("{id:int}/with-offers")]
        public async Task<IActionResult> GetWithOffers(int id)
        {
            var result = await _mediator.Send(new GetServiceRequestWithOffersQuery { ServiceRequestId = id });
            return Ok(result);
        }

        // GET /api/servicerequests/customer/{customerId}
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var result = await _mediator.Send(new GetServiceRequestsByCustomerQuery { CustomerId = customerId });
            return Ok(result);
        }

        // GET /api/servicerequests/open?latitude=...&longitude=...&radiusInMeters=...
        // Technicians discover nearby open requests
        [HttpGet("open")]
        public async Task<IActionResult> GetOpen(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusInMeters = 5000)
        {
            var result = await _mediator.Send(new GetOpenServiceRequestsQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusInMeters = radiusInMeters
            });
            return Ok(result);
        }
    }
}
