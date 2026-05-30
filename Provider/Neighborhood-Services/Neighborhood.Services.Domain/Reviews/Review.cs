using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Neighborhood.Services.Domain.Reviews
{
    public class Review:BaseEntity<int>
    {
        
        public int BookingId { get; set; }
        public int ReviewerId { get; set; }
        public int RevieweeId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public ReviewStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public ReviewAnalysis Analysis { get;  set; }

        // Empty Constructor For EF Core
        private Review() { }

        // Main Constructor — pure data, no defaults
        public Review(
            int bookingId,
            int reviewerId,
            int revieweeId,
            int rating,
            string comment,
            ReviewStatus status,
            DateTime createdAt,
            bool isDeleted)
        {
            BookingId = bookingId;
            ReviewerId = reviewerId;
            RevieweeId = revieweeId;
            Rating = rating;
            Comment = comment;
            Status = status;
            CreatedAt = createdAt;
            IsDeleted = isDeleted;
        }
    }
}