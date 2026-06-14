using MediatR;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Newsletter.Commands
{
    public class DeleteNewsletterCommandDTO : IRequest<CreateNewsCommandDTO>
    {
       public int id { set; get; }

    }//end of dto class

    public class DeleteNewsletterCommandDHandler : IRequestHandler<DeleteNewsletterCommandDTO, CreateNewsCommandDTO>
    {

        private readonly INewsletterRepository _newsrepository;


        private readonly IUnitOfWork _unitOfWork;

        public DeleteNewsletterCommandDHandler(INewsletterRepository newsletter, IUnitOfWork unitOfWork)
        {
            _newsrepository = newsletter;
            _unitOfWork = unitOfWork;

        }//end of const.

        public async Task<CreateNewsCommandDTO> Handle(DeleteNewsletterCommandDTO request, CancellationToken cancellationToken)
        {
            var deleted = await _newsrepository.GetByIdAsync(request.id);
            if (deleted == null) { return null; }

            await _newsrepository.DeleteAsync(request.id);
            await _unitOfWork.SaveChangesAsync();

            //adding email to email addresses group

            return new CreateNewsCommandDTO
            {
                email=deleted.email,
                
            };

        }//end of handling task
    }
}
