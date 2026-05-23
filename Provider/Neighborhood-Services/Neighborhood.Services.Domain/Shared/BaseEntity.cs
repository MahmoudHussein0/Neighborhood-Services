using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Shared
{
    public class BaseEntity <T>
    {
        public T Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
