using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;
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
        public string  CategoryName  { get; set; } 
        public string  CategoryIcon  { get; set; }
        public string ImageUrl { get; set; }
        public List<TechnicianPricingDto> TechnicionPricing { get; set; }

        public ProblemTypeDetailsDto()
        {
            TechnicionPricing = new List<TechnicianPricingDto>();
        }
    }
}
