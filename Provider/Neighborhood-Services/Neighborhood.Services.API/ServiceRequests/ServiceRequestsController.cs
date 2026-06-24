using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Authorization;
using Neighborhood.Services.Application.ServiceRequests.Commands.CreateService;
using Neighborhood.Services.Application.ServiceRequests.Commands.ReviewFlaggedServiceRequest;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetFlaggedServiceRequestsQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetMyServiceRequestsQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetOpenServiceRequestsQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestByIdQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestsByCustomerQuery;
using Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestWithOffersQuery;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.API.ServiceRequests
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        // GET /api/servicerequests/mine?status=Open&search=leak&page=1&pageSize=10
        // (authenticated customer — their own requests, paged + optional filter/search)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine(
            [FromQuery] ServiceRequestStatus? status,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetMyServiceRequestsQuery
            {
                Status = status,
                Search = search,
                Page = page,
                PageSize = pageSize
            });
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

        // ---------- Staff moderation queue ----------

        // GET /api/servicerequests/flagged?page=1&pageSize=10
        // Staff-only: requests the moderation agent flagged as inappropriate.
        [HasPermission(PermissionType.ManageFlagedReq)]
        [HttpGet("flagged")]
        public async Task<IActionResult> GetFlagged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetFlaggedServiceRequestsQuery
            {
                Page = page,
                PageSize = pageSize
            });
            return Ok(result);
        }

        // POST /api/servicerequests/{id}/approve  (Flagged -> Open, goes live)
        [HasPermission(PermissionType.ManageFlagedReq)]
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            await _mediator.Send(new ReviewFlaggedServiceRequestCommand { ServiceRequestId = id, Approved = true });
            return NoContent();
        }

        // POST /api/servicerequests/{id}/reject  (Flagged -> Closed)
        [HasPermission(PermissionType.ManageFlagedReq)]
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            await _mediator.Send(new ReviewFlaggedServiceRequestCommand { ServiceRequestId = id, Approved = false });
            return NoContent();
        }

        // GET /api/servicerequests/open?latitude=...&longitude=...&radiusInMeters=...
        // Technicians discover nearby open requests
        [HttpGet("open")]
        public async Task<IActionResult> GetOpen(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusInMeters = 5000,
            [FromQuery] bool onlyMyCategories = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetOpenServiceRequestsQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusInMeters = radiusInMeters,
                OnlyMyCategories = onlyMyCategories,
                Page = page,
                PageSize = pageSize
            });
            return Ok(result);
        }
    }
}
