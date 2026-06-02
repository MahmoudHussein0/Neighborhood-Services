using MediatR;
using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Newsletter.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Newsletter.Queries
{
    public class GetAllNewsQHandler : IRequestHandler<IRequest<List<CreateNewsCommandDTO>>, List<CreateNewsCommandDTO>>
    {
        private readonly INewsletterRepository _newsrepository;

        public GetAllNewsQHandler(INewsletterRepository newsrepository)
        {
            _newsrepository = newsrepository;
        }
        public async Task<List<CreateNewsCommandDTO>> Handle(IRequest<List<CreateNewsCommandDTO>> request, CancellationToken cancellationToken)
        {
            var items = await _newsrepository.GetAllAsync();
            if (items.Count == 0) { return null; }
            return items.Select(item => new CreateNewsCommandDTO
            {
                id=item.Id,
                email=item.email,
                subscribedAt=item.subscribedAt,
                isDeleted=item.IsDeleted

            })
                .ToList();
        }
    }
}
