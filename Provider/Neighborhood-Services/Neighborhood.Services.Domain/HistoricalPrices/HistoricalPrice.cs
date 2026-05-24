using Neighborhood.Services.Domain.ProblemTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.HistoricalPrices
{
    public class HistoricalPrice
    {
        public int Id { get; set; }
        public int ProblemTypeId { get; set; }
        public string Region { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MaterialCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProblemType ProblemType { get; set; }
    }
}
