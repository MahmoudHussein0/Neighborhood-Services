using Mapster.Utils;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Notifications.Push_inApp.Commands;
using Neighborhood.Services.Application.Notifications.Push_inApp.DTOs;
using Neighborhood.Services.Application.Notifications.Push_inApp.Queries;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Infrastructure.Services.NotificationService;
using System.Drawing;
using System.Net.NetworkInformation;

namespace Neighborhood.Services.API.Controllers.Notification
{

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private IMediator _mediator;
        private INotificationService _service;
        public NotificationsController(IMediator mediator, INotificationService service)
        {
            _mediator = mediator;
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<PushNotificationDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllNotfsQDto());
            return Ok(result);
        }

        [HttpPost("SendingToAll")]
        public async Task<ActionResult> CreateNotificationToAll(string mssg)
        {
            var result = await _service.SendNotificationAsync(mssg);
            return Ok(result);
        }

        [HttpPost("SendingToAUserById/{id}")]
        public async Task<ActionResult> CreateNotificationToUser(string id,string mssg)
        {
            var result = await _service.SendNotificationToUser(id, mssg);
            return Ok(result);
        }

        [HttpPost("SendingToTechnicians")]
        public async Task<ActionResult> CreateNotificationToTechnicians(string mssg)
        {
            var result = await _service.SendNotificationToTechnician(mssg);
            return Ok(result);
        }

        [HttpPost("SendingToCustomers")]
        public async Task<ActionResult> CreateNotificationToCustomers(string mssg)
        {
            var result = await _service.SendNotificationToCustomer(mssg);
            return Ok(result);
        }

        [HttpPost("SendingToAdmins")]
        public async Task<ActionResult> CreateNotificationToAdmins(string mssg)
        {
            var result = await _service.SendNotificationToAdmin(mssg);
            return Ok(result);
        }

        [HttpPost("SendingBasedOnRole")]
        public async Task<ActionResult> CreateNotificationToGroup(string message, string? userRole=null, string? userId = null)
        {
            if (userId != null)
            {
                var result = await _service.SendRoleBasedNotificationAsync(message, ApplicationUserRole.Customer, userId);
                return Ok(result);

            }
            else if (userRole!=null&& Enum.TryParse<ApplicationUserRole>(userRole, out ApplicationUserRole role))
            { 
                var result = await _service.SendRoleBasedNotificationAsync(message, role, userId);
                return Ok(result);
            }
            else
            {
                var result = await _service.SendRoleBasedNotificationAsync(message, ApplicationUserRole.Customer, userId);
                return Ok("Enum Value Not Valid or No userId entered!"); }
        }

        [HttpPut("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead(MarkAllAsReadCommandDto command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("MarkAsRead/{id:int}")]
        public async Task<IActionResult> MarkAsRead(int id, MarkAsReadCommandDto command)
        {
            command.NotificationId = id;
           
            var res=await _mediator.Send(command);
            return Ok(res);
        }

        [HttpGet("GetNotificationsOfCurrentUser")]
        public async Task<ActionResult> GetNotificationsOfCurrentUser()
        {
            var result= await _mediator.Send(new GetAllNotfsForCurrentUserQDto());
            return Ok(result);
        }

        [HttpGet("GetNotificationsByUserId/{userId}")]
        public async Task<ActionResult> GetNotificationsOfUserById(string userId)
        {
            var result = await _mediator.Send(new GetAllNotfsByUserIdQDto() { Id=userId});
            return Ok(result);
        }
    }
}
