using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Shared;
using System.Security.Cryptography;
using System.Text;


namespace Neighborhood.Services.API.Controllers.ChatTest
{
    public class MessageAttachmentDto
    {
       public IFormFile file { set; get; }
         //  public int id { set; get; }
    }

    [Route("api/[controller]")]
    [ApiController]

    public class ChatTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IMediator _mediator;
        private IChatService _service;
        private ICurrentUserService _userService;
       // private Cloudinary _cloudinary = new Cloudinary();
        

        public ChatTestController(
            IMediator mediator, 
            IChatService service, 
            ICurrentUserService userService,
            IConfiguration configuration
            )
        {
            _mediator = mediator;
            _service = service;
            _userService = userService;
            _configuration = configuration;
           
        }

        [HttpPost("SendingToAll")]
        public async Task<ActionResult> CreateNotificationToAll(MessageCreatedDto mssg)
        {
            var result = _service.SendBroadcastMessageDto(mssg);
            return Ok(result);
        }

        //[HttpGet("CurrentUserId")]
        //public async Task<ActionResult> GetCurrentUserId()
        //{
        //    var result = _userService.UserId;
        //    var x = new { result };


        //    return Ok(x);
        //}

        [HttpGet("CurrentUserId")]
        public async Task<IActionResult> GetCurrentUserId()
        {
            var result = _userService.UserId;

            return Ok(new
            {
                userId = result
                
            });
        }


        //المسدج فيها بارامتر hasImaga? true/false
        //لو ترو ==>روح جيبها من الكلاوديناري
        //لو فولس؟ خلاص
        [HttpPost("UploadImage")]

        public async Task<IActionResult> UploadImage([FromForm] MessageAttachmentDto request)
        {
            var account = new Account(
     _configuration["ArwaCloudinary:CloudName"],
    _configuration["ArwaCloudinary:ApiKey"],
     _configuration["ArwaCloudinary:ApiSecret"]);

            var cloudinary = new Cloudinary(account);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(request.file.FileName, request.file.OpenReadStream()),
                UploadPreset = _configuration["ArwaCloudinary:UploadPreset"],
              //  PublicId = request.id.ToString(),
                Folder = "messages",
                Format="jpg"
           
  
            };

            var result = await cloudinary.UploadAsync(uploadParams);


         //   var result = await _cloudinary.UploadAsync(uploadParams);
            return Ok(new { url = result.SecureUrl.ToString() });
        }


        [HttpGet("CloudinarySignature")]
        
        public IActionResult GetSignature()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var apiSecret = _configuration["ArwaCloudinary:ApiSecret"];
            var uploadPreset = _configuration["ArwaCloudinary:UploadPreset"];

           

            var signatureString = $"timestamp={timestamp}&upload_preset={uploadPreset}{apiSecret}";

          
            using var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
            var signature = Convert.ToHexString(hash).ToLower();

            return Ok(new
            {
                signature,
                timestamp,
                apiKey = _configuration["ArwaCloudinary:ApiKey"],
                uploadPreset
            });
        }
    }
}
