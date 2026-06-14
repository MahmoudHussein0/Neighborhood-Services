using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Messages.Queries;
using Neighborhood.Services.Application.Newsletter.Commands;
using Neighborhood.Services.Application.Newsletter.Queries;
using Neighborhood.Services.Application.Shared.Email;
using Neighborhood.Services.Application.Newsletter.DTOs;



namespace Neighborhood.Services.API.Controllers.Newsletter
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterController : ControllerBase
    {
        private IMediator _mediator;
        private readonly IEmailService _emailService;


        public NewsletterController(IMediator mediator, IEmailService emailService)
        {
            _mediator = mediator;
            _emailService= emailService;

        }

        [HttpPost("SendNewsLetter")]
        public async Task<IActionResult> NewsletterEmail([FromBody] NewsletterDto dto)
        {

            var result = await _emailService
        .SendNewsletterEmailAsync(dto.Subj, dto.Content);


            return Ok(result);

        }

        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody]string email)
        {
            var command = new CreateNewsCommandDTO();
            command.email = email;
            var result = await _mediator.Send(command);


            return Ok(result);

        }

        //Get All NewsSubscribers
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var res = await _mediator.Send(new GetAllNewsQDto());
            return Ok(res);
        }
    }
}
