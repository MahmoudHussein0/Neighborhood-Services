using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Conversations.Commands;
using Neighborhood.Services.Application.Shared.Email;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Infrastructure.Services.EmailService;

namespace Neighborhood.Services.API.Controllers.EmailService
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController(IEmailService emailService) : ControllerBase
    {
        private readonly IEmailService _emailService = emailService;

        [HttpPost("no")]
        public async Task<IActionResult> ConfirmBooking(string email, int id, string name, string problem)
        {

            var time = DateTime.UtcNow;
            var result = await _emailService.SendBookingVerificationEmainAsync(email, id, time, problem, name);


            return Ok(result);

        }

        [HttpPut]
        public async Task<IActionResult> ResettingPasswords(string mail, string url)
        {

            var time = DateTime.UtcNow;
            var result = await _emailService.SendPasswordResetEmailAsync(mail, url);


            return Ok(result);

        }

        [HttpPost("hi")]
        public async Task<IActionResult> VerifyEmail(string mail, string url)
        {

            var time = DateTime.UtcNow;
            var result = await _emailService.SendEmailVerificationEmailAsync(mail, url);


            return Ok(result);

        }

        [HttpPost("SendNewsLetter")]
        public async Task<IActionResult> NewsletterEmail(string mail, string url)
        {

            var time = DateTime.UtcNow;
            var result = await _emailService.SendNewsletterEmailAsync(mail, url);


            return Ok(result);

        }

        //SendNewsletterEmailAsync
    }
}
