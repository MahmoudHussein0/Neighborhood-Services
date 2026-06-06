using MediatR;
using Neighborhood.Services.Application.AiAnalysises.DTOs;
using Neighborhood.Services.Application.AiAnalysises.Interface;

namespace Neighborhood.Services.Application.AiAnalysises.Queries.GetAnalysisByBooking
{
    public class GetAnalysisByBookingQuery : IRequest<AiAnalysisDto?>
    {
        public int BookingId { get; set; }
    }

    public class GetAnalysisByBookingQueryHandler : IRequestHandler<GetAnalysisByBookingQuery, AiAnalysisDto?>
    {
        private readonly IAiAnalysisRepository _aiAnalysisRepository;

        public GetAnalysisByBookingQueryHandler(IAiAnalysisRepository aiAnalysisRepository)
        {
            _aiAnalysisRepository = aiAnalysisRepository;
        }

        public async Task<AiAnalysisDto?> Handle(GetAnalysisByBookingQuery request, CancellationToken cancellationToken)
        {
            var analysis = await _aiAnalysisRepository.GetByBookingIdAsync(request.BookingId);
            if (analysis is null)
                return null;

            return new AiAnalysisDto
            {
                DetectedProblem = analysis.DetectedProblem,
                ConfidenceScore = analysis.ConfidenceScore,
                EstimatedMinPrice = analysis.EstimatedMinPrice,
                EstimatedMaxPrice = analysis.EstimatedMaxPrice,
                SeverityLevel = analysis.SeverityLevel,
                GeneratedAt = analysis.GeneratedAt
            };
        }
    }
}
