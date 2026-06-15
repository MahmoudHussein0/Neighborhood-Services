using Neighborhood.Services.Application.ProblemTypes.DTOs;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Domain.TechnicianCategories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public string Image { get; set; }
        public string Icon { get; set; }

        public int Technicians { get; set; }

        public ICollection<ProblemTypeDto> ProblemTypes { get; set; } = new List<ProblemTypeDto>();
    }
}
