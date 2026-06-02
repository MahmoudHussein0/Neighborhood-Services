using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Queries
{
    public class GetSupportMessageByIdQuery : IRequest<SupportMessageDto>
    {
        public int Id { get; set; }
        public GetSupportMessageByIdQuery(int id) => Id = id;
    }
}
