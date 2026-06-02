using Microsoft.AspNetCore.Http;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public string? UserId => _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
