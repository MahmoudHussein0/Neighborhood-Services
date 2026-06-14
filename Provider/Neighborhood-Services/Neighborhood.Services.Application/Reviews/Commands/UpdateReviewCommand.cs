using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Application.Reviews.Commands
{
    public class UpdateReviewCommand : IRequest<ReviewDto>
    {
        public int Id { get; set; }

        // All optional — the staff Reviews tab sends only Status (Approve/Reject/Flag),
        // while a rating/comment edit sends those. Apply whatever is provided.
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public ReviewStatus? Status { get; set; }
    }
}
