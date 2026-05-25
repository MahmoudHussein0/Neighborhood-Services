using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.User.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser,string>
    {
    }
}
