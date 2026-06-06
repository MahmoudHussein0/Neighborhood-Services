using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.CustomerAddresses.Commands;
using Neighborhood.Services.Application.CustomerAddresses.Queries;

namespace Neighborhood.Services.API.CustomerAddresses
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAddressesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Customer,Staff")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Enforce customer-owner-or-Staff before returning address details.
            var result = await _mediator.Send(new GetCustomerAddressByIdQuery { Id = id });
            return Ok(result);
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            // TODO: Enforce customer-owner-or-Staff before returning customer addresses.
            var result = await _mediator.Send(new GetCustomerAddressesByCustomerIdQuery { CustomerId = customerId });
            return Ok(result);
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpGet("user/{applicationUserId}")]
        public async Task<IActionResult> GetByUserId(string applicationUserId)
        {
            // TODO: Enforce self-or-Staff before returning addresses by user id.
            var result = await _mediator.Send(new GetCustomerAddressesByUserIdQuery { ApplicationUserId = applicationUserId });
            return Ok(result);
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpGet("customer/{customerId:int}/default")]
        public async Task<IActionResult> GetDefault(int customerId)
        {
            // TODO: Enforce customer-owner-or-Staff before returning default address.
            var result = await _mediator.Send(new GetDefaultCustomerAddressQuery { CustomerId = customerId });
            return Ok(result);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var result = await _mediator.Send(new GetDeletedCustomerAddressesQuery());
            return Ok(result);
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerAddressCommand command)
        {
            // TODO: Enforce that Customer users can only create addresses for themselves.
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCustomerAddressCommand command)
        {
            // TODO: Enforce customer-owner-or-Staff before updating address.
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpPatch("{id:int}/default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            // TODO: Enforce customer-owner-or-Staff before setting default address.
            await _mediator.Send(new SetDefaultCustomerAddressCommand { Id = id });
            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            await _mediator.Send(new RestoreCustomerAddressCommand { Id = id });
            return NoContent();
        }

        [Authorize(Roles = "Customer,Staff")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Enforce customer-owner-or-Staff before deleting address.
            await _mediator.Send(new DeleteCustomerAddressCommand { Id = id });
            return NoContent();
        }
    }
}
