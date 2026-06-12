using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.TechnicianPhotos.Commands;
using Neighborhood.Services.Application.TechnicianPhotos.Queries;

namespace Neighborhood.Services.API.TechnicianPhotos
{
    [Route("api/technician-photos")]
    [ApiController]
    public class TechnicianPhotosController(
        IMediator mediator,
        IWebHostEnvironment environment) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IWebHostEnvironment _environment = environment;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // TODO: Confirm whether portfolio photo listing should remain public.
            var result = await _mediator.Send(new GetAllTechnicianPhotosQuery());
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Confirm whether portfolio photo details should remain public.
            var result = await _mediator.Send(new GetTechnicianPhotoByIdQuery { Id = id });
            return Ok(result);
        }

        [HttpGet("technician/{technicianId:int}")]
        public async Task<IActionResult> GetByTechnicianId(int technicianId)
        {
            // TODO: Confirm whether technician portfolio listing should remain public.
            var result = await _mediator.Send(new GetTechnicianPhotosByTechnicianIdQuery { TechnicianId = technicianId });
            return Ok(result);
        }

        [Authorize]
        [HttpGet("user/{applicationUserId}")]
        public async Task<IActionResult> GetByUserId(string applicationUserId)
        {
            // TODO: Enforce self-or-Staff before returning photos by user id.
            var result = await _mediator.Send(new GetTechnicianPhotosByUserIdQuery { ApplicationUserId = applicationUserId });
            return Ok(result);
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpPost]
        public async Task<IActionResult> Create(AddTechnicianPhotoCommand command)
        {
            // TODO: Enforce that Technician users can only add photos to their own profile.
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpPost("upload")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> Upload(IFormFile file)
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
                "technician-photos");

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            var photoUrl = $"{Request.Scheme}://{Request.Host}/uploads/technician-photos/{fileName}";
            return Ok(new { PhotoUrl = photoUrl });
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, TechnicianPhotoUpdateRequest request)
        {
            // TODO: Enforce technician-owner-or-Staff before updating photos.
            await _mediator.Send(new UpdateTechnicianPhotoUrlCommand
            {
                Id = id,
                PhotoUrl = request.PhotoUrl
            });

            await _mediator.Send(new UpdateTechnicianPhotoCaptionCommand
            {
                Id = id,
                Caption = request.Caption
            });

            return NoContent();
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpPatch("{id:int}/caption")]
        public async Task<IActionResult> UpdateCaption(int id, UpdateTechnicianPhotoCaptionCommand command)
        {
            // TODO: Enforce technician-owner-or-Staff before updating photos.
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpPatch("{id:int}/url")]
        public async Task<IActionResult> UpdateUrl(int id, UpdateTechnicianPhotoUrlCommand command)
        {
            // TODO: Enforce technician-owner-or-Staff before updating photos.
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Technician,Staff")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Enforce technician-owner-or-Staff before deleting photos.
            await _mediator.Send(new DeleteTechnicianPhotoCommand { Id = id });
            return NoContent();
        }
    }
}
