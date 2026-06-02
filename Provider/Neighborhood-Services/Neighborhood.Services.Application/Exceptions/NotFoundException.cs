using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Exceptions
{
    public class NotFoundException :Exception
    {
        public NotFoundException(string message):base(message)
        {}
        public NotFoundException(string entityName, object key): base($"{entityName} with id '{key}' was not found.")
        { }
    }
}
