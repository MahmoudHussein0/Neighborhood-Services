using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Shared
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}
