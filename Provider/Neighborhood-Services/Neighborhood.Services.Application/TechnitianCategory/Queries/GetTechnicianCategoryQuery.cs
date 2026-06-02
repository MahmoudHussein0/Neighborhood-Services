using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianCategory.Queries
{
    public class GetTechnicianCategoryQuery  : IRequest<IReadOnlyList<CategoryDto>>
    {
        public int TechnicianId { get; set; }
    }
}
