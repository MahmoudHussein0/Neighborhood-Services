using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.HistoricalPrices.DTOs
{
    public class HistoricalPricingDto
    {
        public string ProblemName { get; set; }
        public string ProblemDescription { get; set; }
        public string Region { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MaterialCost { get; set; }

    }

}
