using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Customers.Commands;
using Neighborhood.Services.Application.Customers.Queries;
using Neighborhood.Services.Application.Customers.Queries.GetCurrentCustomerQuery;

namespace Neighborhood.Services.API.Customers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var result = await _mediator.Send(new GetCurrentCustomerQuery());
            return Ok(result);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(result);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _mediator.Send(new GetActiveCustomersQuery());
            return Ok(result);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var result = await _mediator.Send(new GetDeletedCustomersQuery());
            return Ok(result);
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Enforce customer-owner-or-Staff before returning customer details.
            var result = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
            return Ok(result);
        }

        // Public profile (details + stats + approved reviews) — shown when a technician clicks a customer.
        // Open to any authenticated user (technicians need this); does not expose private fields.
        [Authorize]
        [HttpGet("{id:int}/public-profile")]
        public async Task<IActionResult> GetPublicProfile(int id)
        {
            var result = await _mediator.Send(new GetCustomerPublicProfileQuery { CustomerId = id });
            return Ok(result);
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpGet("user/{applicationUserId}")]
        public async Task<IActionResult> GetByUserId(string applicationUserId)
        {
            // TODO: Enforce self-or-Staff before returning customer details by user id.
            var result = await _mediator.Send(new GetCustomerByUserIdQuery { ApplicationUserId = applicationUserId });
            return Ok(result);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
        }

        [Authorize(Roles = "Staff")]
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            await _mediator.Send(new ActivateCustomerCommand { Id = id });
            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _mediator.Send(new DeactivateCustomerCommand { Id = id });
            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            await _mediator.Send(new RestoreCustomerCommand { Id = id });
            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteCustomerCommand { Id = id });
            return NoContent();
        }
    }
}
