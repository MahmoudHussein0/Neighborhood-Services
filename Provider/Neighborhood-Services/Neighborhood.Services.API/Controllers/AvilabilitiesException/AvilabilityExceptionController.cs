using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.AvilabilitiesException.Commands;
using Neighborhood.Services.Application.AvilabilitiesException.DTOs;
using Neighborhood.Services.Application.AvilabilitiesException.Queries;

namespace Neighborhood.Services.API.Controllers.AvilabilitiesException
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvilabilityExceptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AvilabilityExceptionController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AvailiabilityExceptionDTO>>> Get(int technicianId)
             => Ok(await _mediator.Send(new GetAvabilityExceptionForSpecificTechQuery(technicianId) ));



        [HttpPost]
        public async Task<ActionResult<int>> Add(AddAvailabilityExceptionCommand command)
          => Ok(await _mediator.Send(command));



        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update (int id , UpdateAvailabilityExceptionCommand command)
        {
            command.Id = id;
         return  Ok(await _mediator.Send(command));
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete(int id)
            =>  Ok(await _mediator.Send(new DeleteAvailabilityExceptionCommand(id)));





    }
}
