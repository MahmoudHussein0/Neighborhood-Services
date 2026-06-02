using MediatR;
using Neighborhood.Services.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.Commands
{
    public  class AddCategoryCommand :IRequest<int>
    {
        public string Name  { get; set; }
        public string Icon  { get; set; }
    }
}
