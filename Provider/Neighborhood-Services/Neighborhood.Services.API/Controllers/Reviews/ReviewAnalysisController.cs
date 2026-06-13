using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.ReviewsAnalysis.Commands;
using Neighborhood.Services.Application.ReviewsAnalysis.DTOs;
using Neighborhood.Services.Application.ReviewsAnalysis.Queries;

namespace Neighborhood.Services.API.Controllers.Reviews
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewAnalysisController : ControllerBase
    {
    private readonly IMediator _mediator;

    public ReviewAnalysisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReviewAnalysisDto>>> GetAll()
    {
        var result = await _mediator.Send(
            new GetAllReviewAnalysisQuery());

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewAnalysisDto>> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetReviewAnalysisByIdQuery
            {
                Id = id
            });

        return Ok(result);
    }

    [HttpGet("review/{reviewId}")]
    public async Task<ActionResult<ReviewAnalysisDto>> GetByReviewId(int reviewId)
    {
        var result = await _mediator.Send(
            new GetReviewAnalysisByReviewIdQuery
            {
                ReviewId = reviewId
            });

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewAnalysisDto>> Create(
        CreateReviewAnalysisCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }
}
}
