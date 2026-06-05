using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.TechnitianCategory.Commands;
using Neighborhood.Services.Application.TechnitianCategory.Queries;

namespace Neighborhood.Services.API.Controllers.TechnitianCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnitianCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TechnitianCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetById(int id)
            => Ok(await _mediator.Send(new GetTechnicianCategoryQuery(id)));


        [HttpPost]
        public async Task<ActionResult<int>> Add (AddCategoryToTechnicianCommand command)
             => Ok( await _mediator.Send(command));




        [HttpDelete("{technicianId}/{categoryId}")]

        public async Task<ActionResult<int>> Delete (int technicianId , int categoryId)
        {
             return  Ok(await _mediator.Send(new DeleteCategortFromTechnicianCommand(technicianId , categoryId)));
        }


        



    }
}
