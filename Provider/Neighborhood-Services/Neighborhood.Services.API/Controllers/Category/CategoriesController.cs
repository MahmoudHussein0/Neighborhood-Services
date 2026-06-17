using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.API.Helper;
using Neighborhood.Services.Application.Authorization;
using Neighborhood.Services.Application.Cache;
using Neighborhood.Services.Application.Categories.Commands;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Categories.Queries;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Infrastructure.Cache;

namespace Neighborhood.Services.API.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IResponseCacheService _cacheService;

        public CategoriesController(IMediator mediator, IResponseCacheService cacheService)
        {
            _mediator = mediator;
            _cacheService = cacheService;
        }


        // Public read: powers the home-page service carousel and the public Services page,
        // so it must stay anonymous. Admin write ops below keep their permission guard.
        [Cache(600)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll([FromQuery] string? searchTerm, [FromQuery] string lang = "en")
         => Ok(await _mediator.Send(new GetAllCategoriesQuery(lang, searchTerm)));


        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDetailsDto>> GetById(int id, [FromQuery] string lang = "en")
         => Ok(await _mediator.Send(new GetCategoryByIdQuery(id, lang)));


        [HasPermission(PermissionType.ManageCategories)]
        [HttpPost]
        public async Task<ActionResult<int>> Add(AddCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            await _cacheService.RemoveByPatternAsync("/api/Categories*");
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            await _cacheService.RemoveByPatternAsync("/api/Categories*");
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
            => Ok(await _mediator.Send(new DeleteCategoryCommand(id)));
    }
}
