using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.TechnicianCategories
{
    public class TechnicianCategory :BaseEntity<int>
    {
        public int  TechnicianId { get; set; }
        public int  CategoryId { get; set; }
        public Category  Category  { get; set; }
        public Technician Technician  { get; set; }

    }
}
