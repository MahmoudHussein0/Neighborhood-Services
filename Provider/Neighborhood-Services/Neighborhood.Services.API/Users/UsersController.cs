using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Authorization;
using Neighborhood.Services.Application.Users.Commands;
using Neighborhood.Services.Application.Users.Commands.CreateUserCommands;
using Neighborhood.Services.Application.Users.Queries;
using Neighborhood.Services.Application.Users.Queries.GetUserByIdQuery;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.API.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(
        IMediator mediator,
        IWebHostEnvironment environment) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IWebHostEnvironment _environment = environment;

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


        [AllowAnonymous]
        [HttpPost("photo-upload")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file.Length == 0)
            {
                return BadRequest(new { Message = "Image file is required." });
            }

            if (string.IsNullOrWhiteSpace(file.ContentType) ||
                !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { Message = "Only image files are allowed." });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp",
                ".gif"
            };

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { Message = "Unsupported image file type." });
            }

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
                "uploads",
                "user-photos");

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            var photoUrl = $"{Request.Scheme}://{Request.Host}/uploads/user-photos/{fileName}";
            return Ok(new { PhotoUrl = photoUrl });
        }

        [Authorize]
        

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

        [HasPermission(PermissionType.ManageUsers)]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(string id)
        {
            await _mediator.Send(new ActivateUserCommand { Id = id });
            return NoContent();
        }

        [HasPermission(PermissionType.ManageUsers)]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id)
        {
            await _mediator.Send(new DeactivateUserCommand { Id = id });
            return NoContent();
        }

        [HasPermission(PermissionType.ManageUsers)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mediator.Send(new DeleteUserCommand { Id = id });
            return NoContent();
        }
    }
}
