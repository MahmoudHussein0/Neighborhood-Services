using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Knowledge.Commands.ReindexKnowledge;

namespace Neighborhood.Services.API.Knowledge
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KnowledgeController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        // Rebuild the whole vector knowledge index from the DB. Staff-only ops action.
        // Same operation the CLI runs: dotnet run -- reindex-knowledge
        [Authorize(Roles = "Staff")]
        [HttpPost("reindex")]
        public async Task<IActionResult> Reindex()
        {
            await _mediator.Send(new ReindexKnowledgeCommand());
            return Ok(new { message = "Knowledge reindex complete." });
        }
    }
}
