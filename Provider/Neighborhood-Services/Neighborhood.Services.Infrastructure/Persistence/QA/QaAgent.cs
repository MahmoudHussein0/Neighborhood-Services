using MediatR;
using Neighborhood.Services.Application.AI.DTOs;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.QA.DTOs;
using Neighborhood.Services.Application.QA.Interface;
using Neighborhood.Services.Application.ReviewsAnalysis.Commands;
using Neighborhood.Services.Domain.AgentLogs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neighborhood.Services.Infrastructure.Persistence.QA
{
    public class QaAgent : IQaAgent
    {
        private readonly IAiClient _aiClient;
        private readonly IMediator _mediator;

        // The LLM returns sentiment as a string ("Positive"), but ReviewSentiment is an enum.
        // System.Text.Json maps enums from numbers by default, so without this converter the
        // deserialize throws JsonException — which the review handler swallows fail-open,
        // leaving every review stuck in Pending. Case-insensitive to tolerate "positive" too.
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        private const string ReviewSystemPrompt =
                       """
                            You are a review analysis assistant.
                            Analyze the review and return ONLY valid JSON.
                            Schema:
                                  {
                                    "Sentiment":"Positive|Neutral|Negative",
                                    "QualityScore":0,
                                    "FlagAbuse":false
                                 }
                            Rules:
                                    - QualityScore from 0 to 100
                                    - FlagAbuse should be true if offensive language exists
                                    - Return JSON only
                        """;


        private const string DisputeSystemPrompt =
                        """
                            You are a dispute assessment assistant.
                            Analyze the dispute and return ONLY valid JSON.
                              {
                                 "severity": "Low|Medium|High|Critical",
                                 "requiresHumanReview": false,
                              }
                        """;

        public QaAgent(IAiClient aiClient, IMediator mediator)
        {
            _aiClient = aiClient;
            _mediator = mediator;
        }


        public async Task<ReviewAnalysisDto> AnalyzeReviewAsync(string reviewText, int reviewId)
        {
            var response = await _aiClient.CompleteAsync(ReviewSystemPrompt, reviewText, log: new AiCallContext()
            {
                AgentType = AgentType.QA,
                Action = "ReviewAnalysis",
                ReferenceType = AgentLogReferenceType.Review,
                ReferenceId = reviewId
            });

            if (string.IsNullOrWhiteSpace(response))
                throw new BadRequestException("Invalid AI response");
            ReviewAnalysisDto? reviewAnalysisDto;

            // Strip any markdown code fences the model may wrap the JSON in.
            var json = response.Replace("```json", "").Replace("```", "").Trim();

            try
            {
                reviewAnalysisDto = JsonSerializer.Deserialize<ReviewAnalysisDto>(json, JsonOptions);

                if (reviewAnalysisDto is null)
                    throw new Exception("AI returned empty response.");
            }
            catch (JsonException)
            {
                throw new Exception("Invalid AI JSON response.");
            }

            await _mediator.Send(new CreateReviewAnalysisCommand(
                reviewId,
                reviewAnalysisDto.Sentiment,
                reviewAnalysisDto.FlagAbuse,
                reviewAnalysisDto.QualityScore));
            return reviewAnalysisDto;
        }

        public async Task<DisputeAnalysisDto> AnalyzeDisputeAsync(string disputeText, int disputeId)
        {
            var response = await _aiClient.CompleteAsync(DisputeSystemPrompt, disputeText, log: new AiCallContext()
            {
                AgentType = AgentType.QA,
                Action = "DisputeAnalysis",
                ReferenceType = AgentLogReferenceType.Dispute,
                ReferenceId = disputeId
            });
            return JsonSerializer.Deserialize<DisputeAnalysisDto>(response, JsonOptions)!;

        }

    }
}
