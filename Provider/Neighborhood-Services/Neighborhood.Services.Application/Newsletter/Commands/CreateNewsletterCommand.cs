using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Newsletter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Newsletter.Commands
{
    public class CreateNewsCommandDTO : IRequest<CreateNewsCommandDTO>
    {
        public string email { get; set; }
       

    }//end of dto class

    public class CreateNewsletterCommandHandler : IRequestHandler<CreateNewsCommandDTO, CreateNewsCommandDTO>
    {

        private readonly INewsletterRepository _newsrepository;
       

        private readonly IUnitOfWork _unitOfWork;

        public CreateNewsletterCommandHandler(INewsletterRepository newsletter, IUnitOfWork unitOfWork)
        {
            _newsrepository=newsletter;
            _unitOfWork = unitOfWork;
           
        }//end of const.

        public async Task<CreateNewsCommandDTO> Handle(CreateNewsCommandDTO request, CancellationToken cancellationToken)
        {
var news=new Domain.Newsletter.Newsletter() { email=request.email, subscribedAt=DateTime.UtcNow,IsDeleted=false};
           
            await _newsrepository.AddAsync(news);
            await _unitOfWork.SaveChangesAsync();

            //adding email to email addresses group

            return new CreateNewsCommandDTO
            {
               
                email =news.email,
                
               
                
            };

        }//end of handling task
    }
}
