using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.API.Helper;
using Neighborhood.Services.Application.ProblemTypes.Commands;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using Neighborhood.Services.Application.ProblemTypes.Queries;

namespace Neighborhood.Services.API.Controllers.ProblemTypes
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProblemTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [Cache(600)]
        public async Task<ActionResult<ProblemTypeDetailsDto>> GetAll([FromQuery] string? searchTerm, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice , [FromQuery] string lang = "en")
        =>  Ok(await _mediator.Send(new GetAllProblemTypesQuery(lang , searchTerm , minPrice , maxPrice )));
        


        [HttpGet("{id}")]
        public async Task<ActionResult<ProblemTypeDetailsDto>> GetById(int id , string lang = "en")
            => Ok(await _mediator.Send(new GetProblemTypeByIdQuery(lang ,id)));


        [HttpPost]
        public async Task<ActionResult<int>> Add(AddProblemTypeCommand command)
            => Ok(await _mediator.Send(command));


        [HttpPut("{id}")]
        public async Task<ActionResult<ProblemTypeDetailsDto>> Update(int id, UpdateProblemTypeCommand command)
        {
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ProblemTypeDetailsDto>> Delete (int id)
        => Ok(await _mediator.Send(new DeleteProblemTypeCommand(id)));
        

    }
}
