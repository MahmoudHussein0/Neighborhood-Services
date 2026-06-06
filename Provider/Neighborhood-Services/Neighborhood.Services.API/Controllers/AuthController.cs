using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Auth.Commands;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUsers;
using NetTopologySuite.Geometries;
using System.Security.Claims;

namespace Neighborhood.Services.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        IMediator mediator,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var authResponse = await _mediator.Send(command);

            AppendAccessTokenCookie(authResponse.Token, authResponse.ExpiresAt);

            return Ok(new
            {
                authResponse.UserId,
                authResponse.FullName,
                authResponse.Email,
                authResponse.Role,
                authResponse.ExpiresAt
            });
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                GoogleDefaults.AuthenticationScheme,
                redirectUrl);

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return Unauthorized(new { Message = "Google authentication failed" });
            }

            var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { Message = "Google account did not provide an email address" });
            }

            var fullName = loginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
            var user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            var shouldAddExternalLogin = false;

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                shouldAddExternalLogin = user != null;
            }

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = fullName,
                    ApplicationUserRole = ApplicationUserRole.Customer,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RefferalCode = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    Location = new Point(0, 0) { SRID = 4326 }
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        Message = "Failed to create user from Google account",
                        Errors = createResult.Errors.Select(error => error.Description)
                    });
                }

                shouldAddExternalLogin = true;
            }

            if (user.IsDeleted || !user.IsActive)
            {
                return Unauthorized(new { Message = "User account is not active" });
            }

            if (shouldAddExternalLogin)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!addLoginResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        Message = "Failed to link Google account",
                        Errors = addLoginResult.Errors.Select(error => error.Description)
                    });
                }
            }

            var tokenResult = _jwtTokenService.GenerateToken(user);
            AppendAccessTokenCookie(tokenResult.Token, tokenResult.ExpiresAt);

            return Ok(new
            {
                UserId = user.Id,
                user.FullName,
                Email = user.Email ?? string.Empty,
                Role = user.ApplicationUserRole.ToString(),
                tokenResult.ExpiresAt
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            return Ok(new { Message = "Logged out successfully" });
        }

        private void AppendAccessTokenCookie(string token, DateTime expiresAt)
        {
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt
            });
        }
    }
}
