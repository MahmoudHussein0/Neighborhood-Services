using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Authorization;
using Neighborhood.Services.Application.CancellationPolicies.Commands.CreateCancellation;
using Neighborhood.Services.Application.CancellationPolicies.Commands.DeleteCancellation;
using Neighborhood.Services.Application.CancellationPolicies.Commands.UpdateCancellation;
using Neighborhood.Services.Application.CancellationPolicies.Queries.GetAllCancellationPoliciesQuery;
using Neighborhood.Services.Application.CancellationPolicies.Queries.GetCancellationPolicyQuery;
using Neighborhood.Services.Domain.CancellationPolicies;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.API.CancellationPolicies
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationPoliciesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CancellationPoliciesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ---------- Commands (admin only) ----------

        // POST /api/cancellationpolicies
        [HasPermission(PermissionType.ManagePolicies)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCancellationPolicyCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // PUT /api/cancellationpolicies/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCancellationPolicyCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE /api/cancellationpolicies/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteCancellationPolicyCommand { Id = id });
            return NoContent();
        }

        // ---------- Queries ----------

        // GET /api/cancellationpolicies
        [HttpGet]
        [HasPermission(PermissionType.ManagePolicies)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCancellationPoliciesQuery());
            return Ok(result);
        }

        // GET /api/cancellationpolicies/lookup?hoursBeforeBooking=24&appliesTo=Customer
        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup(
            [FromQuery] int hoursBeforeBooking,
            [FromQuery] CancellationPolicyTarget appliesTo)
        {
            var result = await _mediator.Send(new GetCancellationPolicyQuery
            {
                HoursBeforeBooking = hoursBeforeBooking,
                AppliesTo = appliesTo
            });
            return Ok(result);
        }
    }
}
