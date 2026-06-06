using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.TechnitianPricing.Commands;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;
using Neighborhood.Services.Application.TechnitianPricing.Queries;

namespace Neighborhood.Services.API.Controllers.TechnicianPricing
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnicianPricingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TechnicianPricingController(IMediator mediator)
        { 
            _mediator = mediator;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TechnicianPricingDto>> Get(int id ,[FromQuery] string lang = "en")
        => Ok(await _mediator.Send( new GetPricingForProblemTypeQuery(lang , id) ));


        [HttpPost]
        public async Task<ActionResult<int>> Add (AddTechnicianPricingForProblemTypeCommand  command)
        => Ok( await   _mediator.Send(command)) ;



        [HttpPut("{id}")]
        public async Task<ActionResult<UpdatePricingDTO>> Update(int id ,  UpdateTechnicianPricingForProblemTypeCommand command)
        {
          command.Id = id;
          return Ok(await _mediator.Send(command));
        }


        [HttpDelete]
        public async Task<ActionResult<UpdatePricingDTO>> Delete(int id)
            => Ok(await _mediator.Send(new  DeleteTechnicianPricingForProblemTypeCommand (id) ));


    }
}
