using MediatR;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Newsletter.Commands
{
    public class DeleteNotificationCommandDTO : IRequest<CreateNewsCommandDTO>
    {
       public int id { set; get; }

    }//end of dto class

    public class DeleteNewsletterCommandDHandler : IRequestHandler<CreateNewsCommandDTO, CreateNewsCommandDTO>
    {

        private readonly INewsletterRepository _newsrepository;


        private readonly IUnitOfWork _unitOfWork;

        public DeleteNewsletterCommandDHandler(INewsletterRepository newsletter, IUnitOfWork unitOfWork)
        {
            _newsrepository = newsletter;
            _unitOfWork = unitOfWork;

        }//end of const.

        public async Task<CreateNewsCommandDTO> Handle(CreateNewsCommandDTO request, CancellationToken cancellationToken)
        {
            var deleted = await _newsrepository.GetByIdAsync(request.id);
            if (deleted == null) { return null; }

            await _newsrepository.DeleteAsync(request.id);
            await _unitOfWork.SaveChangesAsync();

            //adding email to email addresses group

            return new CreateNewsCommandDTO
            {
                id=deleted.Id,
                email = deleted.email,
                
                subscribedAt = deleted.subscribedAt,
                isDeleted=deleted.IsDeleted
                
            };

        }//end of handling task
    }
}
