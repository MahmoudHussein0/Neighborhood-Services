using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.TechniciansPricing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.DTOs
{
    public  class ProblemTypeDetailsDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public Category Category { get; set; }
        public List<TechnicianPricing> TechnicionPricing { get; set; }

        public ProblemTypeDetailsDto()
        {
            TechnicionPricing = new();
        }
    }
}
