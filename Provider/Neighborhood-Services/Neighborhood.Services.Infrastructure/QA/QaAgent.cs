using Neighborhood.Services.Application.AI.DTOs;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.QA.DTOs;
using Neighborhood.Services.Application.QA.Interface;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.ReviewsAnalysis;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AgentLogs;
using Neighborhood.Services.Domain.AiAnalyses;
using Neighborhood.Services.Domain.Reviews;
using OpenAI.Assistants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Neighborhood.Services.Infrastructure.QA
{
    public class QaAgent : IQaAgent
    {
        private readonly IAiClient _aiClient;
        private readonly IReviewAnalysisRepository _reviewAnalysisRepo;
        private readonly IUnitOfWork _unitOfWork;
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
                                    - ScoreQualty from 0 to 100
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

        public QaAgent(IAiClient aiClient  , IReviewAnalysisRepository reviewAnalysisRepo , IUnitOfWork unitOfWork)
        {
            _aiClient = aiClient;
            _reviewAnalysisRepo = reviewAnalysisRepo;
          _unitOfWork = unitOfWork;
        }


        public async Task<ReviewAnalysisDto> AnalyzeReviewAsync(string reviewText , int reviewId )
        {
         var response = await _aiClient.CompleteAsync(ReviewSystemPrompt, reviewText ,  log: new AiCallContext()
            {
                AgentType = AgentType.QA,
                Action = "ReviewAnalysis",
                ReferenceType = AgentLogReferenceType.Review,
                ReferenceId = reviewId
         });

            if (response == null)
                throw new NotFoundException("Invalid AI Response");

            var reviewAnalysisDto = JsonSerializer.Deserialize<ReviewAnalysisDto>(response)!;
            var reviewAnalysis = new ReviewAnalysis()
            {

                ReviewId = reviewId,
                Sentiment = reviewAnalysisDto.Sentiment ,
                IsFlagged = reviewAnalysisDto.FlagAbuse ,
                QualityScore = reviewAnalysisDto.QualityScore
            };

            await _reviewAnalysisRepo.AddAsync(reviewAnalysis);
            await _unitOfWork.SaveChangesAsync();


            return reviewAnalysisDto;
        }

        public async  Task<DisputeAnalysisDto> AnalyzeDisputeAsync(string disputeText , int disputeId)
        {
            var response = await _aiClient.CompleteAsync(DisputeSystemPrompt, disputeText, log: new AiCallContext()
            {
                AgentType = AgentType.QA,
                Action = "DisputeAnalysis",
                ReferenceType  = AgentLogReferenceType.Dispute, 
                ReferenceId =disputeId
            });
            return JsonSerializer.Deserialize<DisputeAnalysisDto>(response)!;

        }

    }
}
