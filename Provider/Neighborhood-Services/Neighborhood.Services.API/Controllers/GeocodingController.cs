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
            var result = await _geocodingService.GetCoordinatesAsync(address);
            if (result == null)
            {
                return NotFound(new { Message = "No geocoding result found." });
            }

            return Ok(result);
        }

        [HttpGet("reverse")]
        public async Task<IActionResult> Reverse([FromQuery] double lat, [FromQuery] double lng)
        {
            var result = await _geocodingService.GetAddressAsync(lat, lng);
            if (result == null)
            {
                return NotFound(new { Message = "No reverse geocoding result found." });
            }

            return Ok(result);
        }
    }
}
