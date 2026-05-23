using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.DTOs
{
    public class CreateReviewDto
    {
        public int BookingId { get; set; }

        public int ReviewerId { get; set; }

        public int RevieweeId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}
