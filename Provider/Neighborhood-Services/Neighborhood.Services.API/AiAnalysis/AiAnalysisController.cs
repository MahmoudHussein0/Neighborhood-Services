using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.AiAnalysises.Commands.AnalyzeBooking;

using Neighborhood.Services.Application.AiAnalysises.Queries.GetAnalysisByBooking;


namespace Neighborhood.Services.API.AiAnalysis
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiAnalysisController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AiAnalysisController(IMediator mediator)
        {
            _mediator= mediator;
        }
        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody]AnalyzeBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Retrieve a previously saved analysis for a booking
        [HttpGet("{bookingId:int}")]
        public async Task<IActionResult> GetByBooking(int bookingId)
        {
            var result = await _mediator.Send(new GetAnalysisByBookingQuery { BookingId = bookingId });
            if (result is null)
                return NotFound($"No analysis found for booking {bookingId}.");
            return Ok(result);
        }

    }
}
