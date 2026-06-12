using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Users.Commands;
using Neighborhood.Services.Application.Users.Commands.CreateUserCommands;
using Neighborhood.Services.Application.Users.Queries;
using Neighborhood.Services.Application.Users.Queries.GetUserByIdQuery;
using Neighborhood.Services.Domain.ApplicationUsers;

namespace Neighborhood.Services.API.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        //[Authorize(Roles = "Staff")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        //[Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            // TODO: Enforce self-or-Staff ownership before returning user details.
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return Ok(result);
        }

        //[Authorize(Roles = "Staff")]
        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetByRole(ApplicationUserRole role)
        {
            var result = await _mediator.Send(new GetUsersByRoleQuery { ApplicationUserRole = role });
            return Ok(result);
        }

        //[Authorize]
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double distanceInMeters)
        {
            var result = await _mediator.Send(new GetNearbyUsersQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                DistanceInMeters = distanceInMeters
            });

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
        }

        //[Authorize]
        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(string id, UpdateUserProfileCommand command)
        {
            // TODO: Enforce self-or-Staff ownership before updating profile.
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        //[Authorize]
        [HttpPut("{id}/location")]
        public async Task<IActionResult> UpdateLocation(string id, UpdateUserLocationCommand command)
        {
            // TODO: Enforce self-or-Staff ownership before updating location.
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        //[Authorize]
        [HttpPut("{id}/photo")]
        public async Task<IActionResult> UpdatePhoto(string id, UpdateUserPhotoCommand command)
        {
            // TODO: Enforce self-or-Staff ownership before updating photo.
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        //[Authorize(Roles = "Staff")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(string id)
        {
            await _mediator.Send(new ActivateUserCommand { Id = id });
            return NoContent();
        }

        //[Authorize(Roles = "Staff")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id)
        {
            await _mediator.Send(new DeactivateUserCommand { Id = id });
            return NoContent();
        }

        //[Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mediator.Send(new DeleteUserCommand { Id = id });
            return NoContent();
        }
    }
}
