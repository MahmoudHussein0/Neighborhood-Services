using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Neighborhood.Services.Domain.Reviews
{
    public class Review
    {
        public int Id { get; private set; }

        public int BookingId { get; private set; }

        public int ReviewerId { get; private set; }

        public int RevieweeId { get; private set; }
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public int Rating { get; private set; }

        public string Comment { get; private set; }

        public bool IsDeleted { get; private set; }

        public ReviewStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }


        // Navigation Property
        public ReviewAnalysis Analysis { get; private set; }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
