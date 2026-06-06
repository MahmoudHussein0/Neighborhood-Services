using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeocodingController(IGeocodingService geocodingService) : ControllerBase
    {
        private readonly IGeocodingService _geocodingService = geocodingService;

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string address)
        {
            var result = await _geocodingService.GeocodeAsync(address);
            if (result == null)
            {
                return NotFound(new { Message = "No geocoding result found." });
            }

            return Ok(result);
        }
    }
}
