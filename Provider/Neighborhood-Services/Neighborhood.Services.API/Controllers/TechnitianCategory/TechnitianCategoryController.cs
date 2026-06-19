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

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetById( string? lang = "en" )
            => Ok(await _mediator.Send(new GetTechnicianCategoryQuery(lang)));




        [HttpPost]
        public async Task<ActionResult<int>> Add ([FromBody] AddCategoryToTechnicianCommand categoryToTechnicianCommand)
             => Ok( await _mediator.Send(categoryToTechnicianCommand));




        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete(int id)
        {
             return  Ok(await _mediator.Send(new DeleteCategortFromTechnicianCommand(id)));
        }


        



    }
}
