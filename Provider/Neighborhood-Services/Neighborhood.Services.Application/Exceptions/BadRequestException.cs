using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Exceptions
{
    public class BadRequestException:Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
