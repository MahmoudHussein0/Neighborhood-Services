using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Exceptions
{
    public class ForbiddenException:Exception
    {
        public ForbiddenException(string message = "You don't have permission for this action")
    : base(message) { }
    }
}
