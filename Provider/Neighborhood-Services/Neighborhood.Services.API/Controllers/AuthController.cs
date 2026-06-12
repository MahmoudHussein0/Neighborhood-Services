using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Auth.Commands;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Customers;
using NetTopologySuite.Geometries;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Neighborhood.Services.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        IMediator mediator,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        ICustomerRepository customerRepository,
        IConfiguration configuration) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly IConfiguration _configuration = configuration;

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
                authResponse.Photo,
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
                return Redirect(BuildExternalErrorUrl("Google authentication failed."));
            }

            var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return Redirect(BuildExternalErrorUrl("Google account did not provide an email address."));
            }

            var fullName = loginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
            var photo = loginInfo.Principal.FindFirstValue("picture")
                ?? loginInfo.Principal.FindFirstValue("urn:google:picture")
                ?? string.Empty;
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
                    Photo = photo,
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
                    return Redirect(BuildExternalErrorUrl("Failed to create user from Google account."));
                }

                shouldAddExternalLogin = true;
            }
            else if (string.IsNullOrWhiteSpace(user.Photo) && !string.IsNullOrWhiteSpace(photo))
            {
                user.Photo = photo;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            if (user.IsDeleted || !user.IsActive)
            {
                return Redirect(BuildExternalErrorUrl("User account is not active."));
            }

            if (shouldAddExternalLogin)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!addLoginResult.Succeeded)
                {
                    return Redirect(BuildExternalErrorUrl("Failed to link Google account."));
                }
            }

            await EnsureCustomerProfileAsync(user);

            var tokenResult = _jwtTokenService.GenerateToken(user);
            AppendAccessTokenCookie(tokenResult.Token, tokenResult.ExpiresAt);

            var callbackUrl = BuildExternalCallbackUrl(user, tokenResult.ExpiresAt);
            return Redirect(callbackUrl);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new { Message = "Logged out successfully" });
        }

        private void AppendAccessTokenCookie(string token, DateTime expiresAt)
        {
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiresAt
            });
        }

        private async Task EnsureCustomerProfileAsync(ApplicationUser user)
        {
            if (user.ApplicationUserRole != ApplicationUserRole.Customer)
            {
                return;
            }

            if (await _customerRepository.GetByUserIdAsync(user.Id) != null)
            {
                return;
            }

            await _customerRepository.CreateAsync(new Customer
            {
                ApplicationUserId = user.Id,
                IsDeleted = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        private string BuildExternalCallbackUrl(ApplicationUser user, DateTime expiresAt)
        {
            var frontendBaseUrl = _configuration["Frontend:BaseUrl"]
                ?? _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()?.FirstOrDefault()
                ?? "http://localhost:4200";

            var query = new Dictionary<string, string?>
            {
                ["userId"] = user.Id,
                ["fullName"] = user.FullName,
                ["email"] = user.Email ?? string.Empty,
                ["photo"] = user.Photo,
                ["role"] = user.ApplicationUserRole.ToString(),
                ["expiresAt"] = expiresAt.ToString("O")
            };

            var encodedQuery = string.Join("&", query.Select(item =>
                $"{UrlEncoder.Default.Encode(item.Key)}={UrlEncoder.Default.Encode(item.Value ?? string.Empty)}"));

            return $"{frontendBaseUrl.TrimEnd('/')}/auth/external-callback?{encodedQuery}";
        }

        private string BuildExternalErrorUrl(string message)
        {
            var frontendBaseUrl = _configuration["Frontend:BaseUrl"]
                ?? _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()?.FirstOrDefault()
                ?? "http://localhost:4200";

            return $"{frontendBaseUrl.TrimEnd('/')}/auth/login?externalError={UrlEncoder.Default.Encode(message)}";
        }
    }
}
