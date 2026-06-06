using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Technicians.Commands;
using Neighborhood.Services.Application.Technicians.Queries;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.API.Technicians
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechniciansController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllTechniciansQuery());
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetTechnicianByIdQuery { Id = id });
            return Ok(result);
        }

        [Authorize]
        [HttpGet("user/{applicationUserId}")]
        public async Task<IActionResult> GetByUserId(string applicationUserId)
        {
            // TODO: Enforce self-or-Staff ownership before returning technician details by user id.
            var result = await _mediator.Send(new GetTechnicianByUserIdQuery { ApplicationUserId = applicationUserId });
            return Ok(result);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("verification-status/{verificationStatus}")]
        public async Task<IActionResult> GetByVerificationStatus(TechnicianVerificationStatus verificationStatus)
        {
            var result = await _mediator.Send(new GetTechniciansByVerificationStatusQuery { VerificationStatus = verificationStatus });
            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var result = await _mediator.Send(new GetAvailableTechniciansQuery());
            return Ok(result);
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTechnicianCommand command)
        {
            // TODO: Enforce that Technician users can only create their own technician profile.
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateTechnicianCommand command)
        {
            // TODO: Enforce technician-owner-or-Staff before updating technician profile.
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpPatch("{id:int}/availability")]
        public async Task<IActionResult> UpdateAvailability(int id, UpdateTechnicianAvailabilityCommand command)
        {
            // TODO: Enforce technician-owner-or-Staff before updating availability.
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpPatch("{id:int}/verification-status")]
        public async Task<IActionResult> UpdateVerificationStatus(int id, UpdateTechnicianVerificationStatusCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            await _mediator.Send(new ActivateTechnicianCommand { Id = id });
            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _mediator.Send(new DeactivateTechnicianCommand { Id = id });
            return NoContent();
        }

        [Authorize(Roles = "Staff")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteTechnicianCommand { Id = id });
            return NoContent();
        }
    }
}
