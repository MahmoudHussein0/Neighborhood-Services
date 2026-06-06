using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.HistoricalPrices.DTOs;
using Neighborhood.Services.Application.HistoricalPrices.Queries;

namespace Neighborhood.Services.API.Controllers.HistoricalPrices
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoricalPricesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HistoricalPricesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{problemId}")]
        public async Task<ActionResult<IReadOnlyList<HistoricalPricingDto>>> Get (int problemId ,[FromQuery] string lang = "en" )
            => Ok (await _mediator.Send(new GetHistoricalPricesForProblemTypeQuery(lang , problemId)) );

    }
}
