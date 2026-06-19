using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.DTOs
{
    public class ProblemTypeDto
    {
        public int  Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string ImageUrl  { get; set; }
    }
}
