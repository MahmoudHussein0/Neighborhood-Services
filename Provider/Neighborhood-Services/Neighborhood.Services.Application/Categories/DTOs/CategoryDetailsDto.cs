using Neighborhood.Services.Application.ProblemTypes.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.DTOs
{
    public class CategoryDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }

        public List<ProblemTypeDto> ProblemTypes { get; set; }

        public CategoryDetailsDto()
        {
            ProblemTypes = new();
        }

    }
}
