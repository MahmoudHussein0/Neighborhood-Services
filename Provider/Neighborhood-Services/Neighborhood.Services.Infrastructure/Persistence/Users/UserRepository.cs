using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Users
{
    public class UserRepository : GenericRepository<ApplicationUser, string>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
