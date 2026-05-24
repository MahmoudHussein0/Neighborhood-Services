using Neighborhood.Services.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.TechnicionCategories
{
    public class TechnicianCategory
    {
        public int  Id { get; set; }
        public int  TechnicianId { get; set; }
        public int  CategoryId { get; set; }
        public Category  Category  { get; set; }
        //public Technician Technician  { get; set; }

    }
}
