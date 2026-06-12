using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Reviews.Commands;
using Neighborhood.Services.Application.Reviews.Queries;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.API.Controllers.Reviews
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/reviews
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllReviewsQuery(), cancellationToken);
            return Ok(result);
        }

        // GET api/reviews/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetReviewByIdQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpGet("reviewer/{reviewerId}")]
        public async Task<IActionResult> GetByReviewer(string reviewerId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetReviewsByReviewerIdQuery(reviewerId),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("reviewee/{revieweeId}")]
        public async Task<IActionResult> GetByReviewee(string revieweeId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetReviewsByRevieweeIdQuery(revieweeId),
                cancellationToken);

            return Ok(result);
        }
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(ReviewStatus status, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetReviewsByStatusQuery(status),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("flagged")]
        public async Task<IActionResult> GetFlagged(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetFlaggedReviewsQuery(),
                cancellationToken);

            return Ok(result);
        }

        // POST api/reviews
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT api/reviews/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        // DELETE api/reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteReviewCommand { Id = id }, cancellationToken);
            return Ok(result);
        }
    }
}
