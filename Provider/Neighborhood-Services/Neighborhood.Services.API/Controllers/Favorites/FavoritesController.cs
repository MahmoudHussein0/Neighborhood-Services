using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Favorites.Commands;
using Neighborhood.Services.Application.Favorites.Queries;

namespace Neighborhood.Services.API.Controllers.Favorites
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private IMediator _mediator;

        public FavoritesController(IMediator mediator)
        {
            _mediator = mediator;
          
        }
       

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Add(int id)
        {
            AddToFavoriteCommandDto command = new AddToFavoriteCommandDto();
            command.technicianId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateFavoriteCommandDto command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("GetMyFavorites")]
        public async Task<ActionResult> myFavorites()
        {
            var result = await _mediator.Send(new GetCurrentUserFavQDto());
            return Ok(result);
        }

        [HttpGet("GetAllFavoriteItems")]
        public async Task<ActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllFavQDto());
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            return Ok(await _mediator.Send(
                new DeleteFromFavoriteCommandDto { id = id }));
        }
    }
}
