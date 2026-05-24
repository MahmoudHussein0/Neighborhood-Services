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
        public int Rating { get; private set; }
        public string Comment { get; private set; }
        public bool IsDeleted { get; private set; }
        public ReviewStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation Property
        public ReviewAnalysis Analysis { get; private set; }

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