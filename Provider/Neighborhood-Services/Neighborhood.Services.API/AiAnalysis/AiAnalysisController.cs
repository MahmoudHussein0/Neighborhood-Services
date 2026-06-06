using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.AiAnalysises.Commands.AnalyzeBooking;

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
    }
}
