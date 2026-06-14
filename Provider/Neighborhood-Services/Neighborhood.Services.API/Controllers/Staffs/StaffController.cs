using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Authorization;
using Neighborhood.Services.Application.Staffs.Commands;
using Neighborhood.Services.Application.Staffs.Queries;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.API.Controllers.Staffs
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StaffController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/staff
        [HttpGet]
        [HasPermission(PermissionType.ManageUsers)]

        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) 
        {
            var result = await _mediator.Send(new GetAllStaffsQuery(), cancellationToken);
            return Ok(result);
        }

        // GET api/staff/5
        [HttpGet("{id}")]
    
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStaffByIdQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(
    string userId,
    CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetStaffByUserIdQuery(userId),
                cancellationToken);

            return Ok(result);
        }
       
        [HttpGet("active")]
      
        public async Task<IActionResult> GetActive(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetActiveStaffsQuery(),
                cancellationToken);

            return Ok(result);
        }
        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetByRole(
           StaffRole role,
           CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetStaffsByRoleQuery(role),
                cancellationToken);

            return Ok(result);
        }
        // POST api/staff
        [HttpPost]
        [HasPermission(PermissionType.ManageUsers)]

        public async Task<IActionResult> Create(
            CreateStaffCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                command,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }



        // PUT api/staff/5
        [HttpPut("{id}")]
        [HasPermission(PermissionType.ManageUsers)]
      
        public async Task<IActionResult> Update(
            int id,
            UpdateStaffCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;

            var result = await _mediator.Send(
                command,
                cancellationToken);

            return Ok(result);
        }


        // DELETE api/staff/5
        [HttpDelete("{id}")]
        [HasPermission(PermissionType.ManageUsers)]
     
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new DeleteStaffCommand
                {
                    Id = id
                },
                cancellationToken);

            return Ok(result);
        }
    }
}
