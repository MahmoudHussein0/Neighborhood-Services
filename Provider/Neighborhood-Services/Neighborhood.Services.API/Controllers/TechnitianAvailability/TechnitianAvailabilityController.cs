using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Neighborhood.Services.Application.TechnitianAvailability.Commands;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;
using Neighborhood.Services.Application.TechnitianAvailability.Queries;
using System.Globalization;

namespace Neighborhood.Services.API.Controllers.TechnitianAvailability
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnitianAvailabilityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TechnitianAvailabilityController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TechnicianAvailabilityDetailsDTO>>> GetById()
        {
            return Ok(await _mediator.Send(new GetTechAvailabilityForTechnicianQuery()));
        }



        [HttpPost]
        public async Task<ActionResult<int>> Add( AddTechnicianAvailabilityCommand command)
        {

            return await _mediator.Send(command);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<TechnicianAvailabilityDTO>> Update(int id, UpdateTechnicianAvailabilityCommand command)
        {
            command.Id = id;
            return await _mediator.Send(command);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var command = new DeleteTechnicianAvailabilityCommand(id);
            return await _mediator.Send(command);
        }
    }
}
