using MediatR;
using Neighborhood.Services.Application.AI.DTOs;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.AiAnalysises.DTOs;
using Neighborhood.Services.Application.AiAnalysises.Interface;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AgentLogs;
using Neighborhood.Services.Domain.AiAnalyses;
using System.Text.Json;

namespace Neighborhood.Services.Application.AiAnalysises.Commands.AnalyzeBooking
{
    public class AnalyzeBookingCommandHandler : IRequestHandler<AnalyzeBookingCommand, AiAnalysisDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAiAnalysisRepository _aiAnalysisRepository;
        private readonly IAiClient _aiClient;
        private readonly IProblemTypeRepository _problemTypeRepository;

        public AnalyzeBookingCommandHandler(
            IUnitOfWork unitOfWork,
            IAiAnalysisRepository aiAnalysisRepository,
            IAiClient aiClient,
            IProblemTypeRepository problemTypeRepository)
        {
            _unitOfWork = unitOfWork;
            _aiAnalysisRepository = aiAnalysisRepository;
            _aiClient = aiClient;
            _problemTypeRepository = problemTypeRepository;
        }

        public async Task<AiAnalysisDto> Handle(AnalyzeBookingCommand request, CancellationToken cancellationToken)
        {
            // 1. Load problem type for context
            var problemType = await _problemTypeRepository.GetByIdAsync(request.ProblemTypeId);

            // 2. Build prompts
            var systemPrompt = """
                You are a home repair expert. Analyze the provided problem description and image,If the input is in Arabic, respond in Arabic for text fields.
                Return ONLY a valid JSON object with no extra text, in this exact format:
                {
                    "detectedProblem": "string",
                    "confidenceScore": 0.0,
                    "estimatedMinPrice": 0.0,
                    "estimatedMaxPrice": 0.0,
                    "severityLevel": "Low|Medium|High"
                }
                """;

            var userPrompt = $"""

                Problem Type: {problemType?.NameEn} / {problemType?.NameAr}

                Description: {request.Description}
                """;

            // 3. Call the AI
            var rawResponse = await _aiClient.CompleteAsync(systemPrompt, userPrompt, request.ImageUrl, new AiCallContext
            {
                AgentType = AgentType.Booking,
                Action = "AnalyzeBooking",
                ReferenceType = AgentLogReferenceType.Booking,
                ReferenceId = request.BookingId
            });

            // 4. Strip markdown if AI wrapped response in code blocks, then parse
            var json = rawResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var parsed = JsonSerializer.Deserialize<AiAnalysisResult>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new Exception("AI returned an invalid response.");
            // 5. Save to DB (BookingId is optional — null if no booking yet)
            var analysis = new AiAnalysis
            {
                BookingId = request.BookingId,
                DetectedProblem = parsed.DetectedProblem,
                ConfidenceScore = parsed.ConfidenceScore,
                EstimatedMinPrice = parsed.EstimatedMinPrice,
                EstimatedMaxPrice = parsed.EstimatedMaxPrice,
                SeverityLevel = Enum.Parse<SeverityLevel>(parsed.SeverityLevel, ignoreCase: true),
                GeneratedAt = DateTime.UtcNow
            };

            await _aiAnalysisRepository.AddAsync(analysis);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Return DTO
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
