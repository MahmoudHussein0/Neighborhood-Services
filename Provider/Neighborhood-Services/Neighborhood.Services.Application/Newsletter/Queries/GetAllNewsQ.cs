using MediatR;
using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Newsletter.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Newsletter.Queries
{
    public class GetAllNewsQDto : IRequest<List<CreateNewsCommandDTO>>
    {

    }
    public class GetAllNewsQHandler : IRequestHandler<GetAllNewsQDto, List<CreateNewsCommandDTO>>
    {
        private readonly INewsletterRepository _newsrepository;

        public GetAllNewsQHandler(INewsletterRepository newsrepository)
        {
            _newsrepository = newsrepository;
        }
        public async Task<List<CreateNewsCommandDTO>> Handle(GetAllNewsQDto request, CancellationToken cancellationToken)
        {
            var items = await _newsrepository.GetAllAsync();
            if (items.Count == 0) { return null; }
            return items.Select(item => new CreateNewsCommandDTO
            {
                email=item.email

            })
                .ToList();
        }
    }
}
